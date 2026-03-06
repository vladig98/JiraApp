export default interface ErrorToastProps {
    message: string | null;
    onDismiss: () => void;
    autoDismissMs?: number;
}