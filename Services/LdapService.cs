using System.DirectoryServices.Protocols;
using System.Net;

namespace WebApp.Services
{
    /// <summary>
    /// Modelo de usuario obtenido de Active Directory
    /// </summary>
    public class LdapUserInfo
    {
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }

    /// <summary>
    /// Interfaz del servicio LDAP
    /// </summary>
    public interface ILdapService
    {
        Task<LdapUserInfo?> GetUserByUsernameAsync(string username);
        Task<List<LdapUserInfo>> SearchUsersAsync(string searchTerm);
        Task<bool> ValidateCredentialsAsync(string username, string password);
    }

    /// <summary>
    /// Implementación del servicio LDAP para Active Directory
    /// Compatible con Linux y Windows
    /// </summary>
    public class LdapService : ILdapService
    {
        private readonly ILogger<LdapService> _logger;
        
        private readonly string _server;
        private readonly int _port;
        private readonly string _baseDn;
        private readonly string _bindUser;
        private readonly string _bindPassword;
        private readonly string _domain;
        private readonly bool _useSSL;

        public LdapService(ILogger<LdapService> logger)
        {
            _logger = logger;
            
            _server = Environment.GetEnvironmentVariable("LDAP_SERVER") 
                ?? throw new InvalidOperationException("LDAP_SERVER no está configurado en el archivo .env");
            
            _port = int.TryParse(Environment.GetEnvironmentVariable("LDAP_PORT"), out var port) 
                ? port : 389;
            
            _baseDn = Environment.GetEnvironmentVariable("LDAP_BASE_DN") 
                ?? throw new InvalidOperationException("LDAP_BASE_DN no está configurado en el archivo .env");
            
            _bindUser = Environment.GetEnvironmentVariable("LDAP_BIND_USER") 
                ?? throw new InvalidOperationException("LDAP_BIND_USER no está configurado en el archivo .env");
            
            _bindPassword = Environment.GetEnvironmentVariable("LDAP_BIND_PASSWORD") 
                ?? throw new InvalidOperationException("LDAP_BIND_PASSWORD no está configurado en el archivo .env");
            
            _domain = Environment.GetEnvironmentVariable("LDAP_DOMAIN") 
                ?? throw new InvalidOperationException("LDAP_DOMAIN no está configurado en el archivo .env");
            
            _useSSL = bool.TryParse(Environment.GetEnvironmentVariable("LDAP_USE_SSL"), out var ssl) && ssl;
        }

        /// <summary>
        /// Obtiene un usuario específico por su nombre de usuario
        /// </summary>
        public async Task<LdapUserInfo?> GetUserByUsernameAsync(string username)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var connection = CreateConnection();
                    BindToServer(connection);

                    var filter = $"(&(objectClass=user)(sAMAccountName={EscapeLdapFilter(username)}))";
                    var searchRequest = new SearchRequest(
                        _baseDn,
                        filter,
                        SearchScope.Subtree,
                        "sAMAccountName", "givenName", "sn", "mail", "displayName", "department", "title"
                    );

                    var response = (SearchResponse)connection.SendRequest(searchRequest);

                    if (response.Entries.Count > 0)
                    {
                        return MapToLdapUserInfo(response.Entries[0]);
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al buscar usuario {Username} en LDAP", username);
                    throw;
                }
            });
        }

        /// <summary>
        /// Busca usuarios en Active Directory
        /// </summary>
        public async Task<List<LdapUserInfo>> SearchUsersAsync(string searchTerm)
        {
            return await Task.Run(() =>
            {
                var users = new List<LdapUserInfo>();

                try
                {
                    using var connection = CreateConnection();
                    BindToServer(connection);

                    var escapedTerm = EscapeLdapFilter(searchTerm);
                    var filter = $"(&(objectClass=user)(objectCategory=person)(|(sAMAccountName=*{escapedTerm}*)(givenName=*{escapedTerm}*)(sn=*{escapedTerm}*)(displayName=*{escapedTerm}*)(mail=*{escapedTerm}*)))";

                    var searchRequest = new SearchRequest(
                        _baseDn,
                        filter,
                        SearchScope.Subtree,
                        "sAMAccountName", "givenName", "sn", "mail", "displayName", "department", "title"
                    );

                    searchRequest.SizeLimit = 50;

                    var response = (SearchResponse)connection.SendRequest(searchRequest);

                    foreach (SearchResultEntry entry in response.Entries)
                    {
                        var user = MapToLdapUserInfo(entry);
                        if (user != null)
                        {
                            users.Add(user);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al buscar usuarios en LDAP con término: {SearchTerm}", searchTerm);
                    throw;
                }

                return users;
            });
        }

        /// <summary>
        /// Valida las credenciales de un usuario contra Active Directory
        /// </summary>
        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var connection = CreateConnection();
                    
                    // Usar formato UPN o DOMAIN\user para autenticación
                    var bindDn = $"{_domain}\\{username}";
                    var credential = new NetworkCredential(bindDn, password);
                    
                    connection.Credential = credential;
                    connection.AuthType = AuthType.Basic;
                    connection.Bind();
                    
                    return true;
                }
                catch (LdapException ex)
                {
                    _logger.LogWarning("Credenciales inválidas para usuario {Username}: {Message}", username, ex.Message);
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al validar credenciales para {Username}", username);
                    return false;
                }
            });
        }

        private LdapConnection CreateConnection()
        {
            var identifier = new LdapDirectoryIdentifier(_server, _port);
            var connection = new LdapConnection(identifier)
            {
                AuthType = AuthType.Basic
            };

            connection.SessionOptions.ProtocolVersion = 3;
            connection.SessionOptions.ReferralChasing = ReferralChasingOptions.None;

            if (_useSSL)
            {
                connection.SessionOptions.SecureSocketLayer = true;
            }

            return connection;
        }

        private void BindToServer(LdapConnection connection)
        {
            // Formato para autenticación: DOMAIN\user o user@domain.com
            var bindDn = $"{_domain}\\{_bindUser}";
            var credential = new NetworkCredential(bindDn, _bindPassword);
            
            connection.Credential = credential;
            connection.AuthType = AuthType.Basic;
            connection.Bind();
        }

        private static LdapUserInfo? MapToLdapUserInfo(SearchResultEntry entry)
        {
            var username = GetAttributeValue(entry, "sAMAccountName");
            if (string.IsNullOrEmpty(username))
                return null;

            return new LdapUserInfo
            {
                UserName = username,
                FirstName = GetAttributeValue(entry, "givenName") ?? string.Empty,
                LastName = GetAttributeValue(entry, "sn") ?? string.Empty,
                Email = GetAttributeValue(entry, "mail") ?? string.Empty,
                DisplayName = GetAttributeValue(entry, "displayName") ?? string.Empty,
                Department = GetAttributeValue(entry, "department") ?? string.Empty,
                Title = GetAttributeValue(entry, "title") ?? string.Empty
            };
        }

        private static string? GetAttributeValue(SearchResultEntry entry, string attributeName)
        {
            if (entry.Attributes.Contains(attributeName) && entry.Attributes[attributeName].Count > 0)
            {
                return entry.Attributes[attributeName][0]?.ToString();
            }
            return null;
        }

        private static string EscapeLdapFilter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input
                .Replace("\\", "\\5c")
                .Replace("*", "\\2a")
                .Replace("(", "\\28")
                .Replace(")", "\\29")
                .Replace("\0", "\\00");
        }
    }
}
