namespace University.MVC.Models;

public class CardViewModel
{
    public int? Id { get; set; }
    public string Title { get;set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int GroupID { get; set; }
    public int? CourseID { get; set; }

    public string InfoURL { get; set; } = string.Empty;
    public string DownTitle { get; set; } = string.Empty;    
    public string DownURL { get; set; } = string.Empty;

    public int NumPages { get; set; }
}