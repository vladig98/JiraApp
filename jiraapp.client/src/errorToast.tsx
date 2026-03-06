import { useEffect, useState } from 'react'
import type ErrorToastProps from './types/errorToastProps';

export default function ErrorToast({ message, onDismiss, autoDismissMs = 5000 }: ErrorToastProps) {
    const [progress, setProgress] = useState(100);

    useEffect(() => {
        if (!message) {
            return;
        }

        setProgress(100);
        const progressTimer = setTimeout(() => {
            setProgress(0);
        }, 10);

        const dismissTimer = setTimeout(() => {
            onDismiss();
        }, autoDismissMs);

        return () => {
            clearTimeout(progressTimer);
            clearTimeout(dismissTimer);
        };
    }, [message, onDismiss, autoDismissMs]);

    return (
        /* The Container: Fixed positioning with transition logic */
        <div
            className={`fixed bottom-6 right-6 z-50 w-80 bg-white border border-red-200 rounded-lg shadow-xl transform transition-all duration-300 ease-in-out overflow-hidden ${message
                    ? 'opacity-100 translate-y-0 scale-100'
                    : 'opacity-0 translate-y-10 scale-95 pointer-events-none'
                }`}
        >
            {/* Main Content Area */}
            <div className="p-4 flex gap-3 items-start">
                {/* The Error Icon (Warning symbol) */}
                <div className="flex-shrink-0 w-6 h-6 rounded-full bg-red-100 flex items-center justify-center text-red-600 font-bold text-lg">
                    !
                </div>

                {/* The Message Text */}
                <div className="flex-1">
                    <h4 className="text-sm font-semibold text-slate-900">
                        Unexpected Error
                    </h4>
                    <p className="text-sm text-red-700 mt-1 leading-relaxed">
                        {message || "An unexpected network error occurred. No changes made."}
                    </p>
                </div>

                {/* The Close Button */}
                <button
                    onClick={onDismiss}
                    className="flex-shrink-0 text-slate-400 hover:text-slate-600 transition-colors p-1 rounded hover:bg-slate-100"
                    title="Dismiss"
                >
                    &times;
                </button>
            </div>

            {/* The Animated Progress Bar */}
            <div className="h-1 bg-red-100 w-full">
                <div
                    className="h-full bg-red-500 transition-all ease-linear"
                    style={{
                        width: `${progress}%`,
                        transitionDuration: message ? `${autoDismissMs}ms` : '0ms'
                    }}
                />
            </div>
        </div>
    );
}