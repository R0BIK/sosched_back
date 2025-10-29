namespace SoschedBack.Common;

public interface IEndpoint
{
    static abstract IEndpointConventionBuilder Map(IEndpointRouteBuilder app);
}