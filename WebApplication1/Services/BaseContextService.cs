using Data;

namespace WebApplication1.Services
{
    public abstract class BaseContextService
    {
        protected readonly ServiceContext _serviceContext;
        private ServiceContext serviceContext;

        protected BaseContextService(ServiceContext serviceContext)
        {
            _serviceContext = serviceContext;
        }

        protected BaseContextService(ServiceContext serviceContext)
        {
            this._serviceContext = ServiceContext;
        }
    }
}