import {
    app,
    InvocationContext
} from '@azure/functions';

import { isOrder, Order } from '../models/Order';

function parseOrder(message: unknown): Order {
    // Queue triggers may provide JSON messages as
    // deserialized objects rather than raw JSON strings.
    let value: unknown = message;

    if (typeof message === 'string') {
        value = JSON.parse(message);
    }

    if (!isOrder(value)) {
        throw new Error('Invalid order queue message.');
    }

    return value;
}

function processOrder(
    message: unknown,
    context: InvocationContext
): void {

    const order = parseOrder(message);

    context.log(
        `Processing order ${order.orderId} ` +
        `for customer ${order.customerId}`
    );

    // Simulate a processing failure for one specific order.
    if (order.orderId === 'ORD-FAIL') {
        context.warn(
            `Simulating failure for order ${order.orderId}`
        );

        throw new Error(
            `Simulated failure for order ${order.orderId}`
        );
    }

    context.log(
        `Order ${order.orderId} processed successfully`
    );
}

// Register the Azure Storage Queue trigger.
app.storageQueue('ProcessOrder', {
    queueName: 'orders',
    connection: 'AzureWebJobsStorage',
    handler: processOrder
});