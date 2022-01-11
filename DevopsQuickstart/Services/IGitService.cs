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
		void PushToDevopsRepository(DevopsRepository devopsRepository);
	}

	public class GitService : IGitService
	{
		protected readonly GitOptions Options;
		protected readonly Repository Repository;

		public GitService(GitOptions options)
		{
			Options = options;
			
			var directory = Directory.GetCurrentDirectory();
			var repoPath = Repository.Discover(directory) ?? Repository.Init(directory);

			Repository = new Repository(repoPath);
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
			var remote = Repository.Network.Remotes.FirstOrDefault(r => r.Name == "origin");

			if (remote is null)
			{
				return Repository.Network.Remotes.Add("origin", devopsRepository.RemoteUrl);
			}

			Repository.Network.Remotes.Update("origin", r => r.Url = devopsRepository.RemoteUrl);
			
			return remote;
		}
		
		private void CommitChanges()
		{
			Commands.Stage(Repository, "*"); // Stage everything
			Repository.Index.Write();
			
			var committer = new Signature("Devops Quickstart", Options.Username, DateTime.Now);
			Repository.Commit("Quickstart commiting changes", committer, committer);
		}

		private void Push(Remote remote)
		{
			var options = new PushOptions()
			{
				CredentialsProvider = new CredentialsHandler((url, usernameFromUrl, types) => 
					new UsernamePasswordCredentials() 
					{
						Username = Options.Username,
						Password = Options.Password
					})
			};
            
			Repository.Branches.Update(Repository.Head,
				b => b.Remote = remote.Name,
				b => b.UpstreamBranch = Repository.Head.CanonicalName);
            
			Repository.Network.Push(Repository.Head, options);
		}
	}
}