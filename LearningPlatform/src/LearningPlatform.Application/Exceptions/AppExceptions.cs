namespace LearningPlatform.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string entity, object key) : base($"{entity} s ID-em '{key}' nije pronađen.") { }
}

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}

public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "Nemate ovlasti za ovu akciju.") : base(message) { }
}

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}
