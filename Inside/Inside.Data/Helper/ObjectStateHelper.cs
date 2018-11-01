using Inside.Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace Inside.Data.Helper
{
    public class ObjectStateHelper
    {
        public static EntityState ConvertObjectState(ObjectState objectState)
        {
            switch (objectState)
            {
                case ObjectState.Added:
                    return EntityState.Added;
                case ObjectState.Modified:
                    return EntityState.Modified;
                case ObjectState.Deleted:
                    return EntityState.Deleted;
                default:
                    return EntityState.Unchanged;
            }
        }
    }
}