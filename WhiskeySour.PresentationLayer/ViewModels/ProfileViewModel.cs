namespace WhiskeySour.Web.ViewModels;

public class ProfileViewModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public byte[]? ProfilePicture { get; set; }
    public IFormFile? ImageFile { get; set; } //ladda upp ny bild
}