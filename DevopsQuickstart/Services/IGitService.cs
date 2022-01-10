using System;
using System.IO;
using System.Linq;
using DevopsQuickstart.Models.Options;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using DevopsRepository = DevopsQuickstart.Models.Devops.Repository;

namespace DevopsQuickstart.Services
{
	public interface IGitService
	{
		void CreateRepository();
		void PushToDevopsRepository(DevopsRepository devopsRepository);
	}

	public class GitService : IGitService
	{
		private readonly GitOptions _options;
		private Repository _repository;

		public GitService(GitOptions options)
		{
			_options = options;
		}

		public void CreateRepository()
		{
			var directory = Directory.GetCurrentDirectory();
			var repoPath = Repository.Discover(directory) ?? Repository.Init(directory);

			_repository = new Repository(repoPath);
		}

		public void PushToDevopsRepository(DevopsRepository devopsRepository)
		{
			try
			{
				CommitChanges();
			}
			catch
			{
				// Do nothing
			}
			
			var remote = GetRemote(devopsRepository);
			
			Push(remote);
		}

		private Remote GetRemote(DevopsRepository devopsRepository)
		{
			var remote = _repository.Network.Remotes.FirstOrDefault(r => r.Name == "origin");

			if (remote is null)
			{
				return _repository.Network.Remotes.Add("origin", devopsRepository.RemoteUrl);
			}

			_repository.Network.Remotes.Update("origin", r => r.Url = devopsRepository.RemoteUrl);
			
			return remote;
		}
		
		private void CommitChanges()
		{
			Commands.Stage(_repository, "*"); // Stage everything
			_repository.Index.Write();
			
			var committer = new Signature("Devops Quickstart", _options.Username, DateTime.Now);
			_repository.Commit("Quickstart commiting changes", committer, committer);
		}

		private void Push(Remote remote)
		{
			var options = new PushOptions()
			{
				CredentialsProvider = new CredentialsHandler((url, usernameFromUrl, types) => 
					new UsernamePasswordCredentials() 
					{
						Username = _options.Username,
						Password = _options.Password
					})
			};
            
			_repository.Branches.Update(_repository.Head,
				b => b.Remote = remote.Name,
				b => b.UpstreamBranch = _repository.Head.CanonicalName);
            
			_repository.Network.Push(_repository.Head, options);
		}
	}
}