import { useState } from 'react'
import { useNavigate } from '@tanstack/react-router'

export function BoardActionDropdown({ boardId }: { boardId: string }) {
    const [isOpen, setIsOpen] = useState(false);
    const navigate = useNavigate();

    const toggleMenu = (e: React.MouseEvent) => {
        e.preventDefault();
        e.stopPropagation();
        setIsOpen(!isOpen);
    };

    const handleAction = (e: React.MouseEvent, path: 'update' | 'delete') => {
        e.preventDefault();
        e.stopPropagation();
        setIsOpen(false);
        navigate({ to: `/boards/${path}/$boardId`, params: { boardId } });
    };

    return (
        <>
            <button
                onClick={toggleMenu}
                className={`px-2 text-xl leading-none transition-colors rounded ${isOpen ? 'text-slate-900 bg-slate-100' : 'text-slate-400 hover:text-slate-600'
                    }`}
            >
                •••
            </button>

            {isOpen && (
                <>
                    <div
                        className="fixed inset-0 z-10"
                        onClick={() => setIsOpen(false)}
                    />

                    <div className="absolute right-0 mt-2 w-40 bg-white border border-slate-200 rounded-md shadow-lg z-20 overflow-hidden">
                        <button
                            onClick={(e) => handleAction(e, 'update')}
                            className="w-full text-left px-4 py-2 text-sm text-slate-700 hover:bg-slate-50 flex items-center gap-2"
                        >
                            <span className="text-slate-400">✎</span> Update
                        </button>
                        <button
                            onClick={(e) => handleAction(e, 'delete')}
                            className="w-full text-left px-4 py-2 text-sm text-red-600 hover:bg-red-50 flex items-center gap-2"
                        >
                            <span className="text-red-400">🗑</span> Delete
                        </button>
                    </div>
                </>
            )}
        </>
    );
}