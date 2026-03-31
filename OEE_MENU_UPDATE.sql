-- Script para actualizar/crear los menús Paros, OEE y Fallas en la base de datos
-- Ejecutar contra PostgreSQL en la base de datos webapp

-- 1. Obtener el ID del menú Linealytics (si existe)
-- 2. Crear o actualizar el menú Paros
-- 3. Crear o actualizar el menú OEE
-- 4. Crear o actualizar el menú Fallas

DO $$
DECLARE
    v_linealytics_id UUID;
    v_paros_menu_id UUID;
    v_oee_menu_id UUID;
    v_fallas_menu_id UUID;
BEGIN
    -- Obtener el ID del menú Linealytics
    SELECT "Id" INTO v_linealytics_id 
    FROM "menu_items" 
    WHERE "Nombre" = 'Linealytics' AND "ParentId" IS NULL;
    
    IF v_linealytics_id IS NULL THEN
        RAISE NOTICE 'Menú Linealytics no encontrado';
        RETURN;
    END IF;
    
    RAISE NOTICE 'Linealytics ID: %', v_linealytics_id;
    
    -- ============ MENÚ PAROS ============
    -- Buscar si el menú Paros ya existe
    SELECT "Id" INTO v_paros_menu_id 
    FROM "menu_items" 
    WHERE "Nombre" = 'Paros' AND "ParentId" = v_linealytics_id;
    
    IF v_paros_menu_id IS NOT NULL THEN
        -- Actualizar la URL del menú Paros existente
        UPDATE "menu_items" 
        SET "Url" = '~/Linealytics/ParosDirectory', "Orden" = 2, "Activo" = true
        WHERE "Id" = v_paros_menu_id;
        
        RAISE NOTICE 'Menú Paros actualizado: %', v_paros_menu_id;
    ELSE
        -- Crear el menú Paros
        v_paros_menu_id := gen_random_uuid();
        
        INSERT INTO "menu_items" ("Id", "Nombre", "Url", "ParentId", "Orden", "Activo", "FechaCreacion")
        VALUES (v_paros_menu_id, 'Paros', '~/Linealytics/ParosDirectory', v_linealytics_id, 2, true, NOW());
        
        RAISE NOTICE 'Menú Paros creado: %', v_paros_menu_id;
        
        -- Asignar permisos a todos los roles
        INSERT INTO "menu_role_permissions" ("MenuItemId", "RoleId")
        SELECT v_paros_menu_id, "Id"
        FROM "aspnet_roles"
        WHERE "NormalizedName" IN ('SUPERADMIN', 'ADMIN', 'USER')
        ON CONFLICT DO NOTHING;
        
        RAISE NOTICE 'Permisos de Paros asignados a roles';
    END IF;

    -- ============ MENÚ OEE ============
    -- Buscar si el menú OEE ya existe
    SELECT "Id" INTO v_oee_menu_id 
    FROM "menu_items" 
    WHERE "Nombre" = 'OEE' AND "ParentId" = v_linealytics_id;
    
    IF v_oee_menu_id IS NOT NULL THEN
        -- Actualizar la URL del menú OEE existente
        UPDATE "menu_items" 
        SET "Url" = '~/Linealytics/OeeDirectory', "Orden" = 4, "Activo" = true
        WHERE "Id" = v_oee_menu_id;
        
        RAISE NOTICE 'Menú OEE actualizado: %', v_oee_menu_id;
    ELSE
        -- Crear el menú OEE
        v_oee_menu_id := gen_random_uuid();
        
        INSERT INTO "menu_items" ("Id", "Nombre", "Url", "ParentId", "Orden", "Activo", "FechaCreacion")
        VALUES (v_oee_menu_id, 'OEE', '~/Linealytics/OeeDirectory', v_linealytics_id, 4, true, NOW());
        
        RAISE NOTICE 'Menú OEE creado: %', v_oee_menu_id;
        
        -- Asignar permisos a todos los roles
        INSERT INTO "menu_role_permissions" ("MenuItemId", "RoleId")
        SELECT v_oee_menu_id, "Id"
        FROM "aspnet_roles"
        WHERE "NormalizedName" IN ('SUPERADMIN', 'ADMIN', 'USER')
        ON CONFLICT DO NOTHING;
        
        RAISE NOTICE 'Permisos de OEE asignados a roles';
    END IF;

    -- ============ MENÚ FALLAS ============
    -- Buscar si el menú Fallas ya existe
    SELECT "Id" INTO v_fallas_menu_id 
    FROM "menu_items" 
    WHERE "Nombre" = 'Fallas' AND "ParentId" = v_linealytics_id;
    
    IF v_fallas_menu_id IS NOT NULL THEN
        -- Actualizar la URL del menú Fallas existente
        UPDATE "menu_items" 
        SET "Url" = '~/Linealytics/FallasDirectory', "Orden" = 5, "Activo" = true
        WHERE "Id" = v_fallas_menu_id;
        
        RAISE NOTICE 'Menú Fallas actualizado: %', v_fallas_menu_id;
    ELSE
        -- Crear el menú Fallas
        v_fallas_menu_id := gen_random_uuid();
        
        INSERT INTO "menu_items" ("Id", "Nombre", "Url", "ParentId", "Orden", "Activo", "FechaCreacion")
        VALUES (v_fallas_menu_id, 'Fallas', '~/Linealytics/FallasDirectory', v_linealytics_id, 5, true, NOW());
        
        RAISE NOTICE 'Menú Fallas creado: %', v_fallas_menu_id;
        
        -- Asignar permisos a todos los roles
        INSERT INTO "menu_role_permissions" ("MenuItemId", "RoleId")
        SELECT v_fallas_menu_id, "Id"
        FROM "aspnet_roles"
        WHERE "NormalizedName" IN ('SUPERADMIN', 'ADMIN', 'USER')
        ON CONFLICT DO NOTHING;
        
        RAISE NOTICE 'Permisos de Fallas asignados a roles';
    END IF;
END $$;
