export interface Order {
    orderId: string;
    customerId: string;
    amount: number;
}

export function isOrder(value: unknown): value is Order {
    if (
        typeof value !== 'object' ||
        value === null ||
        Array.isArray(value)
    ) {
        return false;
    }

    const candidate = value as Record<string, unknown>;

    return (
        typeof candidate.orderId === 'string' &&
        candidate.orderId.trim().length > 0 &&
        typeof candidate.customerId === 'string' &&
        candidate.customerId.trim().length > 0 &&
        typeof candidate.amount === 'number' &&
        Number.isFinite(candidate.amount) &&
        candidate.amount > 0
    );
}