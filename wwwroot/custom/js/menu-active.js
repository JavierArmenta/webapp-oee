// Menu Active Link Handler
// Automatically marks the current page link as active in the sidebar navigation

document.addEventListener('DOMContentLoaded', function() {
    // Get current page path
    const currentPath = window.location.pathname;
    
    // Get all navigation links in sidebar
    const navLinks = document.querySelectorAll('.nav-sidebar .nav-link');
    
    let activeLink = null;
    let bestMatchLength = 0;
    
    // Find the best matching link (longest path match)
    navLinks.forEach(function(link) {
        const linkPath = link.getAttribute('href');
        
        // Skip empty or hash-only links
        if (!linkPath || linkPath === '#' || linkPath === '') {
            return;
        }
        
        // Normalize paths for comparison
        const normalizedLinkPath = linkPath.replace(/\/$/, ''); // Remove trailing slash
        const normalizedCurrentPath = currentPath.replace(/\/$/, '');
        
        // Check if current path matches or starts with link path
        if (normalizedCurrentPath === normalizedLinkPath || 
            (normalizedCurrentPath.startsWith(normalizedLinkPath + '/') && normalizedLinkPath.length > 1)) {
            
            // Keep track of the longest match (more specific routes have priority)
            if (normalizedLinkPath.length > bestMatchLength) {
                bestMatchLength = normalizedLinkPath.length;
                activeLink = link;
            }
        }
    });
    
    // If we found a matching link, mark it as active
    if (activeLink) {
        // Add active class to the link
        activeLink.classList.add('active');
        
        // Check if the link is inside a submenu
        const parentSubmenu = activeLink.closest('.nav-group-sub');
        if (parentSubmenu) {
            // Get the parent nav-item-submenu container
            const parentNavItem = parentSubmenu.closest('.nav-item-submenu');
            
            if (parentNavItem) {
                // Add the open class
                parentNavItem.classList.add('nav-item-open');
                
                // Show the submenu using Bootstrap Collapse
                const submenuElement = parentNavItem.querySelector('.nav-group-sub');
                if (submenuElement) {
                    submenuElement.classList.add('show');
                }
            }
        }
    }
});
