import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/boards/$boardId')({
    component: () => {
        const { boardId } = Route.useParams()
        return (
            <div className="p-8">
                <h1 className="text-2xl font-bold">Board View: {boardId}</h1>
                <p>This is where the columns and tasks will live.</p>
            </div>
        )
    }
})