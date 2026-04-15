
# GitHub Agentic Workflow

## Purpose
https://github.github.com/gh-aw/

https://github.com/qkfang/ghc-demo-mcp-skill/

https://github.github.com/gh-aw/introduction/overview/


## Demo 2
gh aw add-wizard githubnext/agentics/daily-repo-status


## Demo 3

Initialize this repository for GitHub Agentic Workflows using https://raw.githubusercontent.com/github/gh-aw/main/install.md

Then import and adapt an issue triage workflow from github/gh-aw. Find a suitable issue triage workflow in that repository and adapt it: update the labels, assignee logic, and any repository-specific rules to match this project's conventions.


https://github.github.com/gh-aw/guides/packaging-imports/


# ref

gh extension install github/gh-aw
gh extension upgrade aw

curl -sL https://raw.githubusercontent.com/github/gh-aw/main/install-gh-aw.sh | bash
gh auth login


gh aw add-wizard githubnext/agentics/daily-repo-status
gh aw compile

gh aw status
gh aw run daily-repo-status


