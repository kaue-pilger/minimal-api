using minimal_api.Domain.Enums;

namespace minimal_api.Domain.ModelViews;
public record AdminMV
{
  public int Id { get;set; } = default!;
  public string Email { get;set; } = default!;
  public string Profile { get;set; } = default!;
}
