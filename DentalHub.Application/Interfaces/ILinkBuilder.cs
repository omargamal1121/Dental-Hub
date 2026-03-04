namespace DentalHub.Application.Interfaces
{
    public interface ILinkBuilder
    {
        object GenerateLinks(Guid? id);
        object MakeRelSelf(object links, string apiName);
    }
}
