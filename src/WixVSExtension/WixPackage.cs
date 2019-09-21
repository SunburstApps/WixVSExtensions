using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell;
using WixToolset.VisualStudioExtension.PropertyPages;
using Task = System.Threading.Tasks.Task;

namespace WixToolset.VisualStudioExtension
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    [PackageRegistration(RegisterUsing = RegistrationMethod.CodeBase, UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", ThisAssembly.AssemblyInformationalVersion)]
    [ProvideObject(typeof(WixInstallerPropertyPage), RegisterUsing = RegistrationMethod.CodeBase)]
    [ProvideObject(typeof(WixBuildEventsPropertyPage), RegisterUsing = RegistrationMethod.CodeBase)]
    [ProvideObject(typeof(WixBuildPropertyPage), RegisterUsing = RegistrationMethod.CodeBase)]
    [ProvideObject(typeof(WixPathsPropertyPage), RegisterUsing = RegistrationMethod.CodeBase)]
    [ProvideObject(typeof(WixToolsSettingsPropertyPage), RegisterUsing = RegistrationMethod.CodeBase)]
    [ProvideProjectFactory(typeof(WixProjectFactory), "WiX Toolset", "#100", "wixproj", "wixproj", ".\\NullPath", LanguageVsTemplate = WixProjectNode.ProjectTypeName)]
    [Guid("E0EE8E7D-F498-459E-9E90-2B3D73124AD5")]
    [CLSCompliant(false)]
    public sealed class WixPackage : ProjectPackage
    {
        /// <summary>
        /// Gets the singleton <see cref="WixPackage"/> instance.
        /// </summary>
        public static WixPackage Instance { get; private set; }

        /// <summary>
        /// Gets the settings stored in the Registry for this package.
        /// </summary>
        public WixPackageSettings Settings { get; private set; }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            Instance = this;
            await base.InitializeAsync(cancellationToken, progress).ConfigureAwait(false);

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            this.Settings = new WixPackageSettings(this);
            this.RegisterProjectFactory(new WixProjectFactory(this));
        }
    }
}
