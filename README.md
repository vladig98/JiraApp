# Project name (for clarity): Real-Time Kanban Board

## 1️⃣ Goal of the Application
Build a modern full-stack Kanban board system where multiple users can:
* Create boards
* Create columns
* Create tasks
* Move tasks across columns
* Reorder tasks inside columns
* See updates in real time
* Primary focus: Advanced React state management, async flows, rendering control, and real-time synchronization.

---

## 2️⃣ Tech Stack (Mandatory)

### Backend
* .NET 8
* ASP.NET Core Web API (Minimal APIs allowed)
* Entity Framework Core
* SQLite (or PostgreSQL if preferred)
* SignalR
* FluentValidation
* Swagger

### Frontend
* React 18
* TypeScript
* Vite
* TanStack Query (latest)
* @dnd-kit/core
* @dnd-kit/sortable
* Zustand (or Redux Toolkit — pick one)
* Axios (or native fetch if you prefer)
* No UI frameworks. No component libraries. Keep styling minimal.

---

## 3️⃣ Functional Requirements

### 3.1 Boards
The system must allow:
* Create board
* Rename board
* Delete board
* Fetch all boards
* Select active board
* Each board contains multiple columns.

### 3.2 Columns
Within a board:
* Create column
* Rename column
* Delete column
* Reorder columns
* Column order must persist.

### 3.3 Tasks
Within a column:
* Create task
* Edit task (title + description)
* Delete task
* Reorder tasks within same column
* Move task across columns
* Task order must persist.

### 3.4 Drag & Drop Behavior
The system must support:
* Drag task within column
* Drag task across columns
* Drag column reordering
* Visual feedback while dragging

State must remain consistent after:
* Rapid movements
* Moving back and forth
* Dropping outside valid area

### 3.5 Optimistic Updates
When:
* Creating
* Editing
* Deleting
* Reordering
* Moving

The UI must update immediately before server response.

If server fails:
* State must rollback correctly
* No duplication
* No order corruption

### 3.6 Real-Time Synchronization
Using SignalR:
When one client:
* Creates task
* Moves task
* Edits task
* Deletes task

Other connected clients must update automatically.

Requirements:
* No full-page refresh
* No duplicate items
* Correct reconciliation with optimistic updates
* Consistent ordering across clients

---

## 4️⃣ Non-Functional Requirements

### 4.1 State Management Rules
You must clearly separate server state (Boards, Columns, Tasks) from client state (Active drag item, Modal visibility, Editing state, Temporary UI states).
* Mixing these is not allowed.

### 4.2 Normalized Data Structure
* Frontend must use normalized state structure for tasks and columns.
* No deeply nested arrays for mutation operations.

### 4.3 Rendering Behavior
The application must:
* Avoid unnecessary re-renders
* Ensure task movement does not re-render unrelated columns
* Ensure editing one task does not re-render entire board
* Performance must remain stable with 200+ tasks.

### 4.4 Concurrency Handling
System must handle:
* Two users editing same task
* Two users moving same task
* Fast repeated drag actions
* Slow API responses
* No state corruption allowed.

### 4.5 Error Handling
Must handle:
* API failure
* Network loss
* SignalR disconnection
* UI must not crash.

---

## 5️⃣ API Requirements
Backend must expose endpoints for:
* **Boards**: GET `/boards`, POST `/boards`, PUT `/boards/{id}`, DELETE `/boards/{id}`
* **Columns**: POST `/boards/{boardId}/columns`, PUT `/columns/{id}`, DELETE `/columns/{id}`, PUT `/columns/reorder`
* **Tasks**: POST `/columns/{columnId}/tasks`, PUT `/tasks/{id}`, DELETE `/tasks/{id}`, PUT `/tasks/move`, PUT `/tasks/reorder`
* **SignalR Hub**: Broadcast task changes, Broadcast column reorder, Broadcast task move

---

## 6️⃣ Data Model Requirements
Each entity must contain:
* **Common**: Id (GUID), CreatedAt, UpdatedAt
* **Board**: Name, Order index
* **Column**: BoardId, Name, Order index
* **Task**: ColumnId, Title, Description, Order index, RowVersion (for optimistic concurrency)

---

## 7️⃣ Advanced React Concepts You Must Exercise
This project must force you to use:
* Controlled re-renders
* Memoization strategy
* Derived state (not duplicated state)
* Mutation lifecycle management
* Optimistic UI with rollback
* Separation of concerns
* Custom hooks abstraction
* Async state reconciliation
* Handling stale closures
* Correct dependency management

---

## 8️⃣ Testing Scenarios You Must Validate
Manual scenarios:
* Drag task rapidly between 3 columns
* Open 2 browser tabs and move tasks
* Simulate API delay (2–3 seconds)
* Simulate API failure
* Delete column with many tasks
* Reorder 50 tasks quickly
* Disconnect network mid-drag
* System must remain consistent.

---

## 9️⃣ Project Constraints
* Must work with latest stable .NET 8
* Must use React 18 (not legacy APIs)
* No outdated drag libraries
* No class components
* No legacy Redux patterns
* No CRA

---

## 🔟 Deliverables
* Running backend
* Running frontend
* README explaining: State strategy, Optimistic update approach, Concurrency handling, Real-time reconciliation approach, Rendering optimization decisions

> **Note:** This spec ensures advanced React exposure, real-world async complexity, modern full-stack relevance, deep state reasoning, and interview-ready conceptual mastery.
>
> When you're done building, send the state structure, one mutation flow, one drag flow, and your SignalR integration logic, and I’ll review it like a senior frontend engineer.
