# Project: Advanced Real-Time Kanban Board

## 1️⃣ Goal of the Application
Build a high-performance, real-time Kanban board focusing on advanced frontend architecture. Multiple users can concurrently manage boards, columns, and tasks. 

**Primary Focus:** Mastering React 19 concurrency, advanced state management, optimistic UI rollbacks, handling race conditions, and real-time state reconciliation.

---

## 2️⃣ Tech Stack (Mandatory)

### Backend (The API & Real-Time Hub)
* **.NET 10**
* ASP.NET Core Web API (Minimal APIs preferred)
* Entity Framework Core
* **PostgreSQL 18**
* SignalR (Strictly for Server-to-Client event broadcasting)
* FluentValidation

### Frontend (The Core Challenge)
* **React 19**
* TypeScript (Strict mode enabled)
* Vite
* **TanStack Query v5+** (For server state & caching)
* **@dnd-kit/core & @dnd-kit/sortable** (For complex drag-and-drop interactions)
* **Zustand** (Strictly for client-only UI state)
* Axios or native `fetch`
* **Constraint:** No UI frameworks (e.g., MUI, Chakra). No component libraries. Keep styling minimal (Tailwind or CSS modules are fine).

---

## 3️⃣ Functional Requirements

### 3.1 Core Board Management
* Create, rename, delete, and fetch boards.
* Select an active board (persisted in URL or local storage).
* Create, rename, delete, and reorder columns within a board.

### 3.2 Task Management & Drag-and-Drop
* Create, edit (title/description), and delete tasks.
* **Drag-and-Drop:**
  * Drag tasks within the same column to reorder.
  * Drag tasks across columns.
  * Drag entire columns to reorder the board.
* **Resilience:** The drag state must remain visually consistent even during rapid movements, erratic dragging, or dropping outside valid drop zones.

### 3.3 Advanced Optimistic Updates (React 19 Focus)
Mutations (Create, Edit, Delete, Move, Reorder) must feel instant.
* Use React 19's `useOptimistic` or TanStack Query's `onMutate` to update the UI instantly before the server responds.
* **Rollback:** If the HTTP request fails, the UI must seamlessly revert to its previous state without crashing or requiring a page reload.
* No data duplication or visual flashing during the reconciliation phase.

### 3.4 Real-Time Synchronization Architecture
**Strict Separation of Responsibilities:**
1. **Mutations:** Clients *always* mutate data via HTTP REST endpoints (e.g., `PUT /tasks/move`).
2. **Broadcasting:** The backend processes the HTTP request, saves to PostgreSQL, and then uses SignalR to broadcast the change to *other* clients.
3. **Reconciliation:** When a client receives a SignalR event (e.g., `TaskMovedEvent`), it must cautiously update its TanStack Query cache without triggering unnecessary full-board re-renders.

---

## 4️⃣ Non-Functional Requirements (The "Hard" Part)

### 4.1 Strict State Separation
* **Server State (TanStack Query):** Boards, Columns, Tasks.
* **Client State (Zustand/React Context):** Active drag item data, modal visibility, current editing state, temporary UI filters.
* Mixing these concepts will fail the architectural review.

### 4.2 Rendering Optimization & Concurrency
* **No Unnecessary Re-renders:** Moving a task in Column A must not re-render Column B (unless the task is moved into Column B).
* Utilize React 19's compiler/memoization strategies effectively.
* Performance must remain perfectly smooth (60fps drag-and-drop) even with a board containing **500+ tasks**. Consider utilizing `@tanstack/react-virtual` if necessary.

### 4.3 Concurrency & Conflict Handling
The system must gracefully handle:
* Two users trying to edit or move the exact same task simultaneously (Backend should rely on `RowVersion` for optimistic concurrency; Frontend must handle the 409 Conflict gracefully).
* Slow network connections (simulated 3-second delay) where the user continues dragging other items while a previous move is still pending.

---

## 5️⃣ API & Hub Contract

### REST Endpoints (Source of Truth)
* **Boards:** `GET /boards`, `POST /boards`, `PUT /boards/{id}`, `DELETE /boards/{id}`
* **Columns:** `POST /boards/{boardId}/columns`, `PUT /columns/{id}`, `DELETE /columns/{id}`, `PUT /columns/reorder`
* **Tasks:** `POST /columns/{columnId}/tasks`, `PUT /tasks/{id}`, `DELETE /tasks/{id}`, `PUT /tasks/move`, `PUT /tasks/reorder`

### SignalR Events (Read-Only Broadcasts)
The hub should emit typed events containing the mutated data or ID payloads:
* `ReceiveTaskCreated`, `ReceiveTaskUpdated`, `ReceiveTaskDeleted`
* `ReceiveTaskMoved` (Includes new column and order index)
* `ReceiveColumnReordered`

---

## 6️⃣ Data Model Requirements
All core entities must include:
* `Id` (GUID)
* `CreatedAt` (UTC Timestamp)
* `UpdatedAt` (UTC Timestamp)
* `RowVersion` (byte array/xmin for database concurrency control)

### Relationships
* **Board:** has many Columns.
* **Column:** has many Tasks. Contains an `OrderIndex` (float or lexical string recommended to avoid massive re-indexing on drops).
* **Task:** belongs to a Column. Contains an `OrderIndex`.

---

## 7️⃣ Advanced React Concepts to Demonstrate
This project is designed to evaluate senior-level capabilities. You must demonstrate:
* **React 19 Features:** `useTransition`, `useOptimistic`, or native `use` hooks where applicable.
* **Cache Management:** Direct, precise manipulation of TanStack Query's cache.
* **Stale Closures:** Avoiding stale state inside drag-and-drop event handlers.
* **Lexical Ordering (Optional but impressive):** Implementing Fractional Indexing (e.g., `lexorank`) for ordering tasks to avoid updating multiple rows on a single drag event.

---

## 8️⃣ Mandatory Validation Scenarios
Your application must survive these stress tests:
1. **The Machine Gun:** Rapidly drag a task back and forth between 3 columns before the first API call finishes.
2. **The Multi-Tab:** Open 2 browser tabs. Move a task in Tab A. Ensure Tab B updates seamlessly without completely remounting the board.
3. **The Laggy Connection:** Throttle network to "Slow 3G". Move a task. The UI must instantly reflect the move. If you intentionally fail the API call, the UI must snap back.
4. **The Phantom Edit:** User A is editing a task. User B deletes that task. User A's UI must gracefully handle submitting an edit to a deleted task.

---

## 9️⃣ Deliverables
1. Source code for Backend (.NET 10) and Frontend (React 19).
2. Instructions to run via Docker Compose (should spin up API, Frontend, and Postgres 18).
3. A detailed `README.md` explaining:
   * Your State Management strategy.
   * How you handled optimistic updates + SignalR reconciliation.
   * How you solved rendering performance issues.
   * Why you chose your specific ordering algorithm (integer re-indexing vs. fractional indexing).
