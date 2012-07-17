using System;
using System.Collections.Generic;
using LibGit2Sharp.Core;
using System.Linq;

namespace LibGit2Sharp
{
    /// <summary>
    ///   Provides helper overloads to a <see cref = "Repository" />.
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        ///   Try to lookup an object by its sha or a reference name.
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "repository">The <see cref = "Repository" /> being looked up.</param>
        /// <param name = "shaOrRef">The shaOrRef to lookup.</param>
        /// <returns></returns>
        public static T Lookup<T>(this IRepository repository, string shaOrRef) where T : GitObject
        {
            return (T)repository.Lookup(shaOrRef, GitObject.TypeToTypeMap[typeof(T)]);
        }

        /// <summary>
        ///   Try to lookup an object by its <see cref = "ObjectId" />.
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "repository">The <see cref = "Repository" /> being looked up.</param>
        /// <param name = "id">The id.</param>
        /// <returns></returns>
        public static T Lookup<T>(this IRepository repository, ObjectId id) where T : GitObject
        {
            return (T)repository.Lookup(id, GitObject.TypeToTypeMap[typeof(T)]);
        }

        /// <summary>
        ///   Creates a lightweight tag with the specified name. This tag will point at the commit pointed at by the <see cref = "Repository.Head" />.
        /// </summary>
        /// <param name = "repository">The <see cref = "Repository" /> being worked with.</param>
        /// <param name = "tagName">The name of the tag to create.</param>
        public static Tag ApplyTag(this IRepository repository, string tagName)
        {
            return ApplyTag(repository, tagName, repository.Head.CanonicalName);
        }

        /// <summary>
        ///   Creates a lightweight tag with the specified name. This tag will point at the <paramref name = "target" />.
        /// </summary>
        /// <param name = "repository">The <see cref = "Repository" /> being worked with.</param>
        /// <param name = "tagName">The name of the tag to create.</param>
        /// <param name = "target">The canonical reference name or sha which should be pointed at by the Tag.</param>
        public static Tag ApplyTag(this IRepository repository, string tagName, string target)
        {
            return repository.Tags.Add(tagName, target);
        }

        /// <summary>
        ///   Creates an annotated tag with the specified name. This tag will point at the commit pointed at by the <see cref = "Repository.Head" />.
        /// </summary>
        /// <param name = "repository">The <see cref = "Repository" /> being worked with.</param>
        /// <param name = "tagName">The name of the tag to create.</param>
        /// <param name = "tagger">The identity of the creator of this tag.</param>
        /// <param name = "message">The annotation message.</param>
        public static Tag ApplyTag(this IRepository repository, string tagName, Signature tagger, string message)
        {
            return ApplyTag(repository, tagName, repository.Head.CanonicalName, tagger, message);
        }

        /// <summary>
        ///   Creates an annotated tag with the specified name. This tag will point at the <paramref name = "target" />.
        /// </summary>
        /// <param name = "repository">The <see cref = "Repository" /> being worked with.</param>
        /// <param name = "tagName">The name of the tag to create.</param>
        /// <param name = "target">The canonical reference name or sha which should be pointed at by the Tag.</param>
        /// <param name = "tagger">The identity of the creator of this tag.</param>
        /// <param name = "message">The annotation message.</param>
        public static Tag ApplyTag(this IRepository repository, string tagName, string target, Signature tagger, string message)
        {
            return repository.Tags.Add(tagName, target, tagger, message);
        }

        /// <summary>
        ///   Creates a branch with the specified name. This branch will point at the commit pointed at by the <see cref = "Repository.Head" />.
        /// </summary>
        /// <param name = "repository">The <see cref = "Repository" /> being worked with.</param>
        /// <param name = "branchName">The name of the branch to create.</param>
        public static Branch CreateBranch(this IRepository repository, string branchName)
        {
            return CreateBranch(repository, branchName, repository.Head.CanonicalName);
        }

        /// <summary>
        ///   Creates a branch with the specified name. This branch will point at <paramref name="target"/>.
        /// </summary>
        /// <param name = "repository">The <see cref = "Repository" /> being worked with.</param>
        /// <param name = "branchName">The name of the branch to create.</param>
        /// <param name = "target">The commit which should be pointed at by the Branch.</param>
        public static Branch CreateBranch(this IRepository repository, string branchName, Commit target)
        {
            Ensure.ArgumentNotNull(target, "target");
            return CreateBranch(repository, branchName, target.Id.Sha);
        }

        /// <summary>
        ///   Creates a branch with the specified name. This branch will point at the commit pointed at by the <see cref = "Repository.Head" />.
        /// </summary>
        /// <param name = "repository">The <see cref = "Repository" /> being worked with.</param>
        /// <param name = "branchName">The name of the branch to create.</param>
        /// <param name = "target">The canonical reference name or sha which should be pointed at by the Branch.</param>
        public static Branch CreateBranch(this IRepository repository, string branchName, string target)
        {
            return repository.Branches.Add(branchName, target);
        }

        /// <summary>
        ///   Stores the content of the <see cref = "Repository.Index" /> as a new <see cref = "LibGit2Sharp.Commit" /> into the repository.
        ///   The tip of the <see cref = "Repository.Head"/> will be used as the parent of this new Commit.
        ///   Once the commit is created, the <see cref = "Repository.Head"/> will move forward to point at it.
        ///   <para>Both the Author and Committer will be guessed from the Git configuration. An exception will be raised if no configuration is reachable.</para>
        /// </summary>
        /// <param name = "repository">The <see cref = "Repository" /> being worked with.</param>
        /// <param name = "message">The description of why a change was made to the repository.</param>
        /// <param name = "amendPreviousCommit">True to amend the current <see cref = "LibGit2Sharp.Commit"/> pointed at by <see cref = "Repository.Head"/>, false otherwise.</param>
        /// <returns>The generated <see cref = "LibGit2Sharp.Commit" />.</returns>
        public static Commit Commit(this IRepository repository, string message, bool amendPreviousCommit = false)
        {
            Signature author = BuildSignatureFromGlobalConfiguration(repository, DateTimeOffset.Now);

            return repository.Commit(message, author, amendPreviousCommit);
        }

        /// <summary>
        ///   Stores the content of the <see cref = "Repository.Index" /> as a new <see cref = "LibGit2Sharp.Commit" /> into the repository.
        ///   The tip of the <see cref = "Repository.Head"/> will be used as the parent of this new Commit.
        ///   Once the commit is created, the <see cref = "Repository.Head"/> will move forward to point at it.
        ///   <para>The Committer will be guessed from the Git configuration. An exception will be raised if no configuration is reachable.</para>
        /// </summary>
        /// <param name = "repository">The <see cref = "Repository" /> being worked with.</param>
        /// <param name = "author">The <see cref = "Signature" /> of who made the change.</param>
        /// <param name = "message">The description of why a change was made to the repository.</param>
        /// <param name = "amendPreviousCommit">True to amend the current <see cref = "LibGit2Sharp.Commit"/> pointed at by <see cref = "Repository.Head"/>, false otherwise.</param>
        /// <returns>The generated <see cref = "LibGit2Sharp.Commit" />.</returns>
        public static Commit Commit(this IRepository repository, string message, Signature author, bool amendPreviousCommit = false)
        {
            Signature committer = BuildSignatureFromGlobalConfiguration(repository, DateTimeOffset.Now);

            return repository.Commit(message, author, committer, amendPreviousCommit);
        }

        private static Signature BuildSignatureFromGlobalConfiguration(IRepository repository, DateTimeOffset now)
        {
            var name = repository.Config.Get<string>("user.name", null);
            var email = repository.Config.Get<string>("user.email", null);

            if ((name == null) || (email == null))
            {
                throw new LibGit2SharpException("Can not find Name and Email settings of the current user in Git configuration.");
            }

            return new Signature(name, email, now);
        }

        /// <summary>
        /// Returns the history for the specified file
        /// </summary>
        /// <param name = "repository">The <see cref = "Repository" /> being worked with.</param>
        /// <param name="filePath">The filepath to return the history for</param>
        /// <returns>The calculated list of <see cref=" cref = "LibGit2Sharp.Commit"/>.</returns>
        public static IEnumerable<Commit> History(this Repository repository, string filePath)
        {
            var filter = new Filter
            {
                SortBy = GitSortOptions.Time | GitSortOptions.Reverse
            };

            IndexEntry index = repository.Index[filePath];
            string fileSha;
            //If the index is null, it's probably renamed/deleted
            if (index != null)
            {
                fileSha = index.Id.Sha;
            }
            else
            {
                //TODO : Try find the hard way? i.e. viewing history of file that no longer exists
                throw new LibGit2SharpException(String.Format("Can not find file named '{0}' in the current index.", filePath));
            }

            string path = filePath;
            var returnList = new List<Commit>();

            foreach (Commit commit in repository.Commits.QueryBy(filter))
            {
                //Don't show merges as an individual commit
                if (commit.ParentsCount <= 1)
                {
                    //Search the commit for this filename/sha, using new path if file is renamed
                    path = SearchCommitTree(commit, commit.Tree, fileSha, path, returnList);
                }
            }

            return returnList.OrderBy(commit => commit.Author.When);
        }

        private static string SearchCommitTree(Commit commit, Tree tree, string fileSha, string filePath, List<Commit> changes, bool followRenames = true)
        {
            foreach (var treeItem in tree)
            {
                //If it is a tree rather than a file, iterate through that tree
                if (treeItem.Type == GitObjectType.Tree)
                {
                    var subTree = treeItem.Target as Tree;
                    return SearchCommitTree(commit, subTree, fileSha, filePath, changes);
                }
                else if (treeItem.Type != GitObjectType.Commit)
                {
                    //If the sha and the name are the same then this commit didn't change the file
                    //- Exception to the rule being if this is the first time the file appears in the commit history
                    if (((treeItem.Target.Sha == fileSha) && (treeItem.Path == filePath)) && (changes.Count > 0))
                    {
                        return filePath;
                    }
                    else if ((treeItem.Target.Sha == fileSha) || (treeItem.Path == filePath))
                    {
                        //Sha different, but names the same
                        if ((treeItem.Path == filePath) || (!followRenames))
                        {
                            if (!changes.Contains(commit))
                            {
                                changes.Add(commit);
                            }
                        }
                        else  //Sha the same, but names different
                        {
                            if (!changes.Contains(commit))
                            {
                                changes.Add(commit);
                            }

                            //Merge handling - make sure this rename wasn't part of a merge otherwise a commit step is lost
                            foreach (var parent in commit.Parents)
                            {
                                string path = treeItem.Path;
                                return SearchCommitTree(parent, parent.Tree, treeItem.Target.Sha, path, changes, false);
                            }
                        }
                    }
                }
            }

            return filePath;
        }
    }
}
