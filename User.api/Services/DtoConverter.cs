namespace User_API.Services;

using System;
using System.Reflection;

public abstract class DtoConverter<T, TU> where T: class where TU: class 
{
    public static void UpdateWithDto(T entity, TU entityDto)
    {
        var entityType = typeof(T);
        var entityDtoType = typeof(TU);

        foreach (var entityDtoProp in entityDtoType.GetProperties())
        {
            PropertyInfo? entityProp;
            
            if (entityDtoProp.Name == "Password")
            {
                entityProp = entityType.GetProperty("HashPassword");
                
                if (entityProp != null && entityProp.CanWrite)
                {
                    var value = entityDtoProp.GetValue(entityDto);
                
                    if (value == null)
                        continue;
                
                    entityProp.SetValue(entity, BCrypt.Net.BCrypt.HashPassword(value.ToString()));
                }
            }
            else
            {
                entityProp = entityType.GetProperty(entityDtoProp.Name);
                
                if (entityProp != null && entityProp.CanWrite)
                {
                    var value = entityDtoProp.GetValue(entityDto);
                
                    if (value == null)
                        continue;
                
                    entityProp.SetValue(entity, value);
                }
            }
        }
    }
    
    public static TU ToDto(T entity)
    {
        var entityType = typeof(T);
        var entityDtoType = typeof(TU);
        
        var constructor = entityDtoType.GetConstructors().FirstOrDefault();
        
        if (constructor == null)
            throw new InvalidOperationException("DTO class must have a constructor");

        var parameters = constructor.GetParameters();
        var args = new object[parameters.Length];
        
        for (int i = 0; i < parameters.Length; i++)
        {
            var param = parameters[i];
            
            var entityProp = entityType.GetProperty(param.Name!, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            
            if (entityProp == null)
                continue;
            
            args[i] = entityProp.GetValue(entity)!;
        }
        
        var dto = (TU)constructor.Invoke(args);

        return dto;
    }
}
