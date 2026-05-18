
# GitHub Agentic Workflow

## Purpose

Main page
https://github.github.com/gh-aw/

More use cases
https://github.github.com/gh-aw/introduction/overview/

Repo
https://github.com/qkfang/ghc-demo-mcp-skill/



## Demo 1
Show how the daily-activity-report agent looks like in github
https://github.com/qkfang/ghc-demo-mcp-skill/actions/workflows/daily-activity-report.lock.yml

## Setup

```
gh extension remove github/gh-aw
gh extension install github/gh-aw --pin v0.68.3
gh extension upgrade aw

curl -sL https://raw.githubusercontent.com/github/gh-aw/main/install-gh-aw.sh | bash
gh auth login
```

## Demo 2
Now lets create above AW 
gh aw add-wizard githubnext/agentics/daily-repo-status

## Demo 3
Now lets create a triage agent.
https://github.com/qkfang/ghc-demo-mcp-skill/actions/workflows/issue-triage.lock.yml
https://github.com/qkfang/ghc-demo-mcp-skill/issues

```
Initialize this repository for GitHub Agentic Workflows using https://raw.githubusercontent.com/github/gh-aw/main/install.md

Then import and adapt an issue triage workflow from github/gh-aw. Find a suitable issue triage workflow in that repository and adapt it: update the labels, assignee logic, and any repository-specific rules to match this project's conventions.


https://github.github.com/gh-aw/guides/packaging-imports/
```

# Reference

gh aw add-wizard githubnext/agentics/daily-repo-status
gh aw compile

gh aw status
gh aw run daily-repo-status


