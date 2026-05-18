# GHC Demo Guide

---

## Demo 1: Build a React Website

**Prompt:**

> Build a website for lego robotics team.
> The website should use react and only 3 pages: home, about, robotics.
> Use port 3001, and use current folder as project root.

**Setup commands:**

```bash
npm install
set PORT=3001 && npm start
```

**Follow-up prompt:**

> I don't like the red button, change it to blue.

---

## Demo 1b: Copilot Instructions

**File:** `.github/copilot-instructions.md`

**Global instruction:**

```markdown
the background of the website must be light gray (#f5f5f5)
```

**File-scoped instruction (frontmatter):**

```markdown
---
applyTo: '*.html'
---

must include a <meta> tag with name="author"
must include a <meta> tag with name="description"
```

---

## Demo 2: Figma Integration

**Figma design link:**

<https://www.figma.com/design/eZQZjcT4yMO1UcXbttvNtI/Lego-Website?node-id=1-629&t=eZgnBbB7yoFTBn1o-4>

**Prompt:**

> Make a HTML webpage based on this design. Must include images, and keep it simple.
> https://www.figma.com/design/eZQZjcT4yMO1UcXbttvNtI/Lego-Website?node-id=1-629&t=eZgnBbB7yoFTBn1o-4

**Start Figma MCP server:**

```bash
npx figma-developer-mcp --figma-api-key=figd_85DgxTNr8nKUWdL3AIS6grHZo-xxxxx
```

**MCP config** (`.vscode/mcp.json`):

```json
{
  "servers": {
    "figma": {
      "command": "npx",
      "args": ["-y", "figma-developer-mcp", "--stdio"],
      "env": {
        "FIGMA_API_KEY": "${input:figma_api}"
      }
    }
  },
  "inputs": [
    {
      "id": "figma_api",
      "type": "promptString",
      "description": "Figma API Key",
      "password": true
    }
  ]
}
```

---

## Demo 3: GitHub Copilot Spaces

**Spaces URL:** <https://github.com/copilot/spaces>

**Prompts:**

> What GHC spaces can you see?

> Use GHC spaces to write python to move lego robot forward 10cm.

**MCP config** (`.vscode/mcp.json`):

```json
{
  "servers": {
    "github": {
      "type": "http",
      "url": "https://api.githubcopilot.com/mcp/",
      "headers": {
        "X-MCP-Toolsets": "default,copilot_spaces"
      }
    }
  }
}
```

---

## Demo 4: GitHub MCP Server

**MCP config** (`.vscode/mcp.json`):

```json
{
  "servers": {
    "github": {
      "type": "http",
      "url": "https://api.githubcopilot.com/mcp/"
    }
  }
}
```

**Prompts:**

> What issues do I have in my GitHub repo?

> Create a new issue titled: make the website prettier.

---

## Demo 5: Lego Robot Commands

**Prompts:**

> Move forward 10 cm.

> Move forward 10cm and backward 20cm.

> Open grabber and then close grabber, finally turn around.

> Do a zig zag move backward 50cm.

---

## Demo 6: SpecKit

**Install UV:**

```powershell
powershell -ExecutionPolicy ByPass -c "irm https://astral.sh/uv/install.ps1 | iex"
```

**Navigate to project:**

```bash
cd C:\demo\ghc-demo\demo-project-speckit
```

**Initialize:**

```bash
specify init legorobot
```

**SpecKit slash commands:**

```
/speckit.constitution keep it simple and only create html web pages.
/speckit.specify Build a simple html web for lego robotics team
/speckit.plan the website has a home page
/speckit.tasks
/speckit.implement
```


