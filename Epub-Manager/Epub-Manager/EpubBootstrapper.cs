using Caliburn.Micro;
using Castle.Windsor;
using Castle.Windsor.Installer;
using DevExpress.Xpf.Core;
using Epub_Manager.Views.Shell;
using Epub_Manager.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Epub_Manager
{
    public class EpubBootstrapper : BootstrapperBase
    {
        IWindsorContainer _container;

        public EpubBootstrapper()
        {
            this.Initialize();
        }

        protected override void Configure()
        {
            this._container = new WindsorContainer();
            this._container.Install(FromAssembly.This());

            this.ConfigureDevExpressTheme();
        }

        protected override object GetInstance(Type service, string key)
        {
            if (this._container.Kernel.HasComponent(service) == false)
                return base.GetInstance(service, key);

            return this._container.Resolve(service);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            if (this._container.Kernel.HasComponent(service) == false)
                return base.GetAllInstances(service);

            return this._container.ResolveAll(service).Cast<object>();
        }

        protected override void BuildUp(object instance)
        {
            IEnumerable<PropertyInfo> propertiesToInject = instance
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(f => f.CanWrite && this._container.Kernel.HasComponent(f.PropertyType));

            foreach (var property in propertiesToInject)
            {
                property.SetValue(instance, this._container.Resolve(property.PropertyType));
            }
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            this.DisplayRootViewFor<ShellViewModel>(WindowSettings.With().FixedSize(1280, 720).NoIcon());
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            this._container.Dispose();
        }

        private void ConfigureDevExpressTheme()
        {
            ThemeManager.ApplicationThemeName = "Office2013";
        }
    }
}