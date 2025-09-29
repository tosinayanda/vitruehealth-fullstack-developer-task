namespace Suggestions.API.Shared.Constants;

public static class Routes
{
    public static class Departments
    {
        public const string Base = "departments";
        public const string All = Base;
        public const string Create = Base;
        public const string GetById = Base + "/{id:int}";
    }

    public static class Suggestions
    {
        public const string Base = "suggestions";
        public const string All = Base;
        public const string Create = Base;
        public const string Update = Base + "/{id:Guid}";
        public const string GetById = Base + "/{id:Guid}";
        public const string BulkCreateOrUpdate = Base + "/bulk";
    }

    public static class Employees
    {
        public const string Base = "employees";
        public const string All = Base;
        public const string Create = Base;
        public const string GetById = Base + "/{id:Guid}";
    }

    public static class Users
    {
        public const string Base = "users";
        public const string Create = Base;
        public const string GetById = Base + "/{id:int}";
    }
}
