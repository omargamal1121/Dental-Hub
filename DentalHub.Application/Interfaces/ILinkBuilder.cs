namespace DentalHub.Application.Interfaces
{
    public interface ILinkBuilder
    {
        object GenerateLinks(int? id);
        object MakeRelSelf(object links, string apiName);
    }
}
