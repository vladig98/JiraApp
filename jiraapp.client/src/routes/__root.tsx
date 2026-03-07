import { createRootRoute, Link, Outlet } from '@tanstack/react-router'

export const Route = createRootRoute({
    component: () => (
        <div className="flex h-screen bg-white font-sans text-slate-800">
            <aside className="w-64 bg-[#0747A6] text-white flex flex-col shrink-0">
                <div className="p-6 text-xl font-bold flex items-center gap-2">
                    <div className="w-8 h-8 bg-white rounded-md flex items-center justify-center text-[#0747A6]">J</div>
                    <span>Boards</span>
                </div>
                <nav className="mt-4 flex-1">
                    <Link to="/">
                        <div className="px-6 py-2 bg-white/10 border-r-4 border-white">Boards</div>
                    </Link>
                </nav>
            </aside>

            <main className="flex-1 flex flex-col overflow-hidden">
                <Outlet />
            </main>
        </div>
    ),
})