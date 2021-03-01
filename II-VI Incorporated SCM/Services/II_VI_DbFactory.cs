using System;
using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using II_VI_Incorporated_SCM.Models;

namespace II_VI_Incorporated_SCM.Services
{
    public class II_VI_DbFactory
    {
        public static void Builder()
        {
            SetAutofacContainer();
        }

        private static void SetAutofacContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<DbFactory>().As<IDbFactory>().InstancePerRequest();
            builder.RegisterType<CCNService>().As<ICCNService>().InstancePerRequest();
            builder.RegisterType<NCRManagementService>().As<INCRManagementService>().InstancePerRequest();
            builder.RegisterType<ChangeItemService>().As<IChangeItemService>().InstancePerRequest();
            builder.RegisterType<ReciverService>().As<IReciverService>().InstancePerRequest();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();
            builder.RegisterType<EmailService>().As<IEmailService>().InstancePerRequest();
            builder.RegisterType<SCARService>().As<ISCARService>().InstancePerRequest();
            builder.RegisterType<TaskManagementService>().As<ITaskManagementService>().InstancePerRequest();
            builder.RegisterType<ReportNcrService>().As<IReportNcrService>().InstancePerRequest();
            builder.RegisterType<MeetingNoteService>().As<IMeetingNoteService>().InstancePerRequest();
            builder.RegisterType<ESuggestionService>().As<IESuggestionService>().InstancePerRequest();
            builder.RegisterType<ProductTranferService>().As<IProductTranferService>().InstancePerRequest();
            builder.RegisterType<SoReviewService>().As<ISoReviewService>().InstancePerRequest();
            builder.RegisterType<HomeService>().As<IHomeService>().InstancePerRequest();
            
            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
    public interface IDbFactory : IDisposable
    {
        IIVILocalDB Init();
    }

    public class DbFactory : Disposable, IDbFactory
    {
        private IIVILocalDB dbContext;

        public IIVILocalDB Init()
        {
            return dbContext ?? (dbContext = new IIVILocalDB());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
    public class Disposable : IDisposable
    {
        private bool isDisposed;

        ~Disposable()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!isDisposed && disposing)
            {
                DisposeCore();
            }

            isDisposed = true;
        }

        // Ovveride this to dispose custom objects
        protected virtual void DisposeCore()
        {
        }
    }
}