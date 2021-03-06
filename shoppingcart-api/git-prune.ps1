Param
(
        [Parameter(Mandatory = $false)][switch]$remote
)

# garbage collect
git gc --aggressive

# update local list of pruned branches on the remote to local:
git fetch --prune 

if ($remote.IsPresent) {
	# delete branches on remote origin that have been merge to master
	git branch --merged remotes/origin/master -r | %{$_.trim().replace('origin/', '')} | ?{$_ -notmatch 'master'} | ?{$_ -notmatch 'develop'} | ?{$_ -notmatch 'release/*'} | %{ "delete remote $_"; git push --delete origin $_ }
} else {
	# delete local branches that have been merged to master
	git branch --merged remotes/origin/master | %{$_.trim()} | ?{$_ -notmatch 'master'} | ?{$_ -notmatch 'develop'} | ?{$_ -notmatch 'release/*'} | %{ "delete local $_"; git branch -d $_ }
}

# remove stale refs (local refs to branches that are gone on the remote)
git remote prune origin

# garbage collect
git gc --aggressive
