using ServiceStack;
using BlazorDiffusion.ServiceModel;

namespace BlazorDiffusion.ServiceInterface;

public class MyServices : Service
{
    public object Any(Hello request)
    {
        return new HelloResponse { Result = $"Hello, {request.Name}!" };
    }
}