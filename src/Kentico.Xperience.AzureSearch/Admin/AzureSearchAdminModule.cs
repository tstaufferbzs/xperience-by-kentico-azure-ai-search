﻿using CMS;
using CMS.Base;
using CMS.Core;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.AzureSearch.Admin;
using Kentico.Xperience.AzureSearch.Indexing;
using Microsoft.Extensions.DependencyInjection;

[assembly: RegisterModule(typeof(AzureSearchAdminModule))]

namespace Kentico.Xperience.AzureSearch.Admin;

/// <summary>
/// Manages administration features and integration.
/// </summary>
internal class AzureSearchAdminModule : AdminModule
{
    private IAzureSearchConfigurationStorageService storageService = null!;
    private AzureSearchModuleInstaller installer = null!;

    public AzureSearchAdminModule() : base(nameof(AzureSearchAdminModule)) { }

    protected override void OnInit(ModuleInitParameters parameters)
    {
        base.OnInit(parameters);

        RegisterClientModule("kentico", "xperience-integrations-azuresearch");

        var services = parameters.Services;

        installer = services.GetRequiredService<AzureSearchModuleInstaller>();
        storageService = services.GetRequiredService<IAzureSearchConfigurationStorageService>();

        ApplicationEvents.Initialized.Execute += InitializeModule;
    }

    private void InitializeModule(object? sender, EventArgs e)
    {
        installer.Install();

        AzureSearchIndexStore.SetIndicies(storageService);
    }
}
