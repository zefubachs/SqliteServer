using SqliteServer.Modules.Database.Models;
using FluentValidation;

namespace SqliteServer.Modules.Database.Validators;

public class CreateDatabaseValidator : AbstractValidator<CreateDatabaseRequest>
{
    public CreateDatabaseValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Naam is verplicht")
            .NotEqual("master", StringComparer.InvariantCultureIgnoreCase).WithMessage("Mag naam 'master' niet gebruiken");
    }
}
