import {
    app,
    HttpRequest,
    HttpResponseInit,
    InvocationContext,
    output
} from '@azure/functions';

import { isOrder } from '../models/Order';

// Configure the Azure Storage Queue output binding.
const orderQueue = output.storageQueue({
    queueName: 'orders',
    connection: 'AzureWebJobsStorage'
});

// Define the HTTP function handler.
async function submitOrder(
    request: HttpRequest,
    context: InvocationContext
): Promise<HttpResponseInit> {

    let body: unknown;

    // Deserialize the incoming JSON request.
    try {
        body = await request.json();
    } catch {
        return {
            status: 400,
            jsonBody: {
                error: 'Invalid JSON request.'
            }
        };
    }

    // Validate the incoming order.
    if (!isOrder(body)) {
        return {
            status: 400,
            jsonBody: {
                error: 'OrderId, CustomerId, and a positive Amount are required.'
            }
        };
    }

    const order = body;

    context.log(`Queuing order ${order.orderId}`);

    // Write the serialized order using the queue output binding.
    context.extraOutputs.set(
        orderQueue,
        JSON.stringify(order)
    );

    // Return HTTP 202 Accepted.
    return {
        status: 202,
        jsonBody: {
            orderId: order.orderId,
            status: 'Queued'
        }
    };
}

// Register the function using the Node.js v4 programming model.
app.http('SubmitOrder', {
    methods: ['POST'],
    authLevel: 'anonymous',
    route: 'orders',
    extraOutputs: [orderQueue],
    handler: submitOrder
});