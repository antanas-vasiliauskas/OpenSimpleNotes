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

    //public Guid BusinessId { get; init; }

    public bool IsDeleted { get; init; }


    //public virtual BusinessDao Business { get; init; } = null!;
    //public virtual ICollection<ReservationDao> Reservations { get; init; } = [];
    //public virtual ICollection<RefundDao> Refunds { get; init; } = [];
    //public virtual ICollection<GiftCardDao> GiftCards { get; init; } = [];
    //public virtual ICollection<OrderDao> Orders { get; init; } = [];
}