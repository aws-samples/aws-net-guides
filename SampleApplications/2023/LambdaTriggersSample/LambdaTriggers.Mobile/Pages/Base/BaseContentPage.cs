namespace LambdaTriggers.Mobile;

abstract class BaseContentPage<T> : ContentPage where T : BaseViewModel
{
	protected BaseContentPage(T viewModel, string pageTitle)
	{
		base.BindingContext = viewModel;

		Padding = 12;
		Title = pageTitle;
	}

	protected new T BindingContext => (T)base.BindingContext;
}