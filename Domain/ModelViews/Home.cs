namespace minimal_api.Domain.ModelViews;

public struct Home
{
    public string Message { get => "Welcome!"; }   
    public string Docs { get => "/swagger"; }   
}
