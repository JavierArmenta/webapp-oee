-- Script para verificar que las tablas de fallas existan
-- Ejecute esto en su base de datos PostgreSQL

-- Verificar si existe la tabla CatalogoFallas
SELECT EXISTS (
   SELECT FROM information_schema.tables
   WHERE table_schema = 'linealytics'
   AND table_name = 'CatalogoFallas'
) AS "CatalogoFallas_existe";

-- Verificar si existe la tabla RegistrosFallas
SELECT EXISTS (
   SELECT FROM information_schema.tables
   WHERE table_schema = 'linealytics'
   AND table_name = 'RegistrosFallas'
) AS "RegistrosFallas_existe";

-- Ver estructura de CatalogoFallas (si existe)
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_schema = 'linealytics'
AND table_name = 'CatalogoFallas'
ORDER BY ordinal_position;

-- Ver estructura de RegistrosFallas (si existe)
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_schema = 'linealytics'
AND table_name = 'RegistrosFallas'
ORDER BY ordinal_position;

-- Contar registros existentes
SELECT
    (SELECT COUNT(*) FROM linealytics."CatalogoFallas") AS catalogos_count,
    (SELECT COUNT(*) FROM linealytics."RegistrosFallas") AS registros_count;
