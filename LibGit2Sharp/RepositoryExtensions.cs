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
                SortBy = GitSortOptions.Time
            };

            //Look the file up in the current index
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

            var commit = repository.Commits.QueryBy(filter).First();
            FollowCommitByFile(repository, commit, fileSha, filePath, returnList);

            return returnList.OrderBy(c => c.Author.When);
        }

        private static void FollowCommitByFile(Repository repository, Commit commit, string fileSha, string filePath, List<Commit> changes)
        {
            string path = filePath;
            string sha = fileSha;

            foreach (var parent in commit.Parents)
            {
                //Only track the commit if it wasn't a merge, otherwise we track the commits that make up the merge
                if (commit.ParentsCount == 1)
                {
                    var diff = repository.Diff.Compare(parent.Tree, commit.Tree, new[] { filePath });
                    //If the file change, track it
                    if (diff.Count() > 0)
                    {
                        if (!changes.Contains(commit))
                        {
                            changes.Add(commit);
                            sha = diff.First().Oid.Sha;
                            path = diff.First().OldPath;

                            //If it was an add or delete, do a check for renames
                            if ((diff.Deleted.Count() > 0) || (diff.Added.Count() > 0))
                            {
                                var foundPath = ManualTreeCompare(parent, parent.Tree, sha, path);
                                if (!String.IsNullOrEmpty(foundPath))
                                {
                                    path = foundPath;
                                }
                                else  //Sha or name wasnt found, do a % diff check
                                {
                                    //Get the current file so we can compare it to files changed in diff
                                    var blob = commit.Tree[path].Target as Blob;

                                    if (blob != null)
                                    {
                                        //Get the total lines so we can work out percentage
                                        var lineCount = repository.Diff.Compare(repository.Commits.Last().Tree, commit.Tree, new[] { path });
                                        var manualDiffPath = FindByDiffPercentage(repository, parent.Tree, blob, lineCount.LinesAdded);
                                        if (!String.IsNullOrEmpty(manualDiffPath))
                                        {
                                            path = manualDiffPath;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                FollowCommitByFile(repository, parent, sha, path, changes);
            }

            //Check if this file existed in the initial commit
            if (commit.ParentsCount == 0)
            {
                if (!String.IsNullOrEmpty(ManualTreeCompare(commit, commit.Tree, sha, path)))
                {
                    if (!changes.Contains(commit))
                    {
                        changes.Add(commit);
                    }
                }
            }

        }

        private static string FindByDiffPercentage(Repository repository, Tree tree, Blob latestBlob, int lineCount, int threshold = 50)
        {
            foreach (var treeEntry in tree)
            {
                if (treeEntry.Type == GitObjectType.Tree)
                {
                    var found = FindByDiffPercentage(repository, treeEntry.Target as Tree, latestBlob, lineCount);

                    if (!String.IsNullOrEmpty(found))
                    {
                        return found;
                    }
                }
                else if (treeEntry.Type == GitObjectType.Blob)
                {
                    var diff = repository.Diff.Compare(treeEntry.Target as Blob, latestBlob);
                    if (!diff.IsBinaryComparison)
                    {
                        var percentage = ((double)lineCount / (diff.LinesAdded + diff.LinesDeleted + lineCount)) * 100;
                        if (percentage >= threshold)
                        {
                            return treeEntry.Path;
                        }
                    }
                }
            }

            //File not found in this tree
            return null;
        }

        private static string ManualTreeCompare(Commit commit, Tree tree, string fileSha, string filePath)
        {
            foreach (var treeEntry in tree)
            {
                if (treeEntry.Type == GitObjectType.Tree)
                {
                    var found = ManualTreeCompare(commit, treeEntry.Target as Tree, fileSha, filePath);

                    if ((!String.IsNullOrEmpty(found)) && (found != filePath))
                    {
                        return found;
                    }
                }
                else if (treeEntry.Type != GitObjectType.Commit)
                {
                    if ((treeEntry.Target.Sha == fileSha) || (treeEntry.Path == filePath))
                    {
                        return treeEntry.Path;
                    }
                }
            }

            //File not found in this tree
            return null;
        }
    }
}
