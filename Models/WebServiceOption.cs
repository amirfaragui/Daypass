namespace ValueCards.Models
{
  public class Credential
  {
    public string Username { get; set; }
    public string Password { get; set; }
  }
  public class WebServiceOption
  {
    public string Url { get; set; }
    public Credential Credential { get; set; }
  }
}
