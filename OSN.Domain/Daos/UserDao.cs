using Microsoft.EntityFrameworkCore;
using OSN.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;


namespace OSN.Domain.Daos;

[Index(nameof(Email), IsUnique = true)]
public record UserDao
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = null!;

    [Column(TypeName = "smallint")]
    public UserRole Role { get; init; }

    public bool IsDeleted { get; init; }


    public virtual ICollection<NoteDao> Notes { get; init; } = [];
}