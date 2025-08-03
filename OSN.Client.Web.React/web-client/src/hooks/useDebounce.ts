import { useCallback, useRef, useEffect } from 'react';

export type DebouncedFunction<T extends (...args: any[]) => any> = T & {
    flush: () => void;
    cancel: () => void;
};

export function useDebounce<T extends (...args: any[]) => any>(
    fn: T,
    delay: number
): DebouncedFunction<T> {
    const timer = useRef<number | undefined>(undefined);
    const lastArgs = useRef<Parameters<T> | null>(null);

    const cancel = useCallback(() => {
        if (timer.current) {
            window.clearTimeout(timer.current);
            timer.current = undefined;
        }
    }, []);

    const flush = useCallback(() => {
        if (lastArgs.current) {
            fn(...lastArgs.current);
            lastArgs.current = null;
            cancel();
        }
    }, [fn, cancel]);

    const debouncedFn = useCallback((...args: Parameters<T>) => {
        cancel();
        lastArgs.current = args;

        timer.current = window.setTimeout(() => {
            if (lastArgs.current) {
                fn(...lastArgs.current);
                lastArgs.current = null;
            }
        }, delay);
    }, [fn, delay, cancel]);

    useEffect(() => () => cancel(), [cancel]);

    return Object.assign(debouncedFn, { flush, cancel }) as DebouncedFunction<T>;
}
