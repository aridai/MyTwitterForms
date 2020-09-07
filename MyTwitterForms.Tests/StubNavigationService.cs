
using System;
using System.Threading.Tasks;
using Prism.Navigation;

namespace MyTwitterForms.Tests
{
    internal class StubNavigationService : INavigationService
    {
        public Task<INavigationResult> GoBackAsync() =>
            Task.FromResult<INavigationResult>(new StubNavigationResult());

        public Task<INavigationResult> GoBackAsync(INavigationParameters parameters) =>
            Task.FromResult<INavigationResult>(new StubNavigationResult());

        public Task<INavigationResult> NavigateAsync(Uri uri) =>
            Task.FromResult<INavigationResult>(new StubNavigationResult());

        public Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters) =>
            Task.FromResult<INavigationResult>(new StubNavigationResult());

        public Task<INavigationResult> NavigateAsync(string name) =>
            Task.FromResult<INavigationResult>(new StubNavigationResult());

        public Task<INavigationResult> NavigateAsync(string name, INavigationParameters parameters) =>
            Task.FromResult<INavigationResult>(new StubNavigationResult());
    }
}
