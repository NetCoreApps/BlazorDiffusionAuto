using BlazorDiffusion.ServiceModel;
using ServiceStack;

namespace BlazorDiffusion.ServiceInterface;

public class KamalUpService
{
    public object Get(KamalUp request)
    {
        return new KamalUpResponse();
    }
}