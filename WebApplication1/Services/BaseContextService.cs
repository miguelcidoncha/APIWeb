using Data;

namespace WebApiEcommerce.Services
{
    public abstract class BaseContextService
    {
        protected readonly ServiceContext _serviceContext;
        protected BaseContextService(ServiceContext serviceContext)
        {
            _serviceContext = serviceContext;
        }
    }
}