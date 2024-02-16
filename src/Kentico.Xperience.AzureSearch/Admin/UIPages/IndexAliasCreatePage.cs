﻿using CMS.Membership;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.AzureSearch.Admin;
using Kentico.Xperience.AzureSearch.Admin.UIPages;
using Kentico.Xperience.AzureSearch.Aliasing;
using Kentico.Xperience.AzureSearch.Indexing;
using IFormItemCollectionProvider = Kentico.Xperience.Admin.Base.Forms.Internal.IFormItemCollectionProvider;

[assembly: UIPage(
   parentType: typeof(IndexAliasListingPage),
   slug: "create",
   uiPageType: typeof(IndexAliasCreatePage),
   name: "Create alias",
   templateName: TemplateNames.EDIT,
   order: UIPageOrder.NoOrder)]

namespace Kentico.Xperience.AzureSearch.Admin;

[UIEvaluatePermission(SystemPermissions.CREATE)]
internal class IndexAliasCreatePage : BaseIndexAliasEditPage
{
    private readonly IPageUrlGenerator pageUrlGenerator;
    private AzureSearchAliasConfigurationModel? model = null;

    public IndexAliasCreatePage(
        IFormItemCollectionProvider formItemCollectionProvider,
        IFormDataBinder formDataBinder,
        IAzureSearchIndexAliasService azureSearchIndexAliasService,
        IAzureSearchConfigurationStorageService storageService,
        IPageUrlGenerator pageUrlGenerator)
        : base(formItemCollectionProvider, formDataBinder, azureSearchIndexAliasService, storageService) => this.pageUrlGenerator = pageUrlGenerator;

    protected override AzureSearchAliasConfigurationModel Model
    {
        get
        {
            model ??= new();

            return model;
        }
    }

    protected override async Task<ICommandResponse> ProcessFormData(AzureSearchAliasConfigurationModel model, ICollection<IFormItem> formItems)
    {
        var result = await ValidateAndProcess(model);

        if (result.ModificationResult == ModificationResult.Success)
        {
            var alias = AzureSearchIndexAliasStore.Instance.GetRequiredAlias(model.AliasName);

            var successResponse = NavigateTo(pageUrlGenerator.GenerateUrl<IndexAliasEditPage>(alias.Identifier.ToString()))
                .AddSuccessMessage("Index alias created.");

            return successResponse;
        }

        var errorResponse = ResponseFrom(new FormSubmissionResult(FormSubmissionStatus.ValidationFailure));

        if (result.ErrorMessages is not null)
        {
            result.ErrorMessages.ForEach(errorMessage => errorResponse.AddErrorMessage(errorMessage));
        }
        else
        {
            errorResponse.AddErrorMessage("Could not create index alias.");
        }

        return errorResponse;
    }
}
