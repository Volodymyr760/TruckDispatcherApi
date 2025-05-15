namespace TruckDispatcherApi.Data
{
    public class CreateEntityFailedException : Exception
    {
        public CreateEntityFailedException(string email, Type entity, Exception innerException)
            : base(string.Format("Failed to create {0} with email {1}", entity.Name, email), innerException) { }

        public CreateEntityFailedException(Type entity, Exception innerException)
            : base(string.Format("Failed to create {0}", entity.Name, string.Empty), innerException) { }
    }

    public class DeleteEntityFailedException : Exception
    {
        public DeleteEntityFailedException(int id, Type entity, Exception innerException)
            : base(string.Format("Failed to delete {0} {1}", entity.Name, id), innerException) { }

        public DeleteEntityFailedException(string id, Type entity, Exception innerException)
            : base(string.Format("Failed to delete {0} {1}", entity.Name, id), innerException) { }

        public DeleteEntityFailedException(Type entity, Exception innerException)
            : base(string.Format("Failed to delete {0}", entity.Name), innerException) { }
    }

    public class HttpResponseException : Exception
    {
        public int Status { get; set; } = 500;

        public object Value { get; set; }
    }

    public class RetrieveEntityFailedException<TEntity> : Exception
    {
        public RetrieveEntityFailedException(string id, Exception innerException)
            : base(string.Format("Failed to retrieve {0} {1}", typeof(TEntity).Name, id), innerException) { }
    }

    public class RetrieveEntitiesQueryFailedException : Exception
    {
        public RetrieveEntitiesQueryFailedException(Type entites, Exception innerException)
            : base(string.Format("Failed query to get all {0}", entites.Name), innerException) { }
    }

    public class UpdateEntityFailedException : Exception
    {
        public UpdateEntityFailedException(Type entity, Exception innerException)
            : base(string.Format("Failed to update {0}", entity.Name), innerException) { }

        public UpdateEntityFailedException(int id, Type entity, Exception innerException)
            : base(string.Format("Failed to update {0} {1}", entity.Name, id), innerException) { }

        public UpdateEntityFailedException(string id, Type entity, Exception innerException)
            : base(string.Format("Failed to update {0} {1}", entity.Name, id), innerException) { }
    }
}
