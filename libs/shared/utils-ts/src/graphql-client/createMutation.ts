import { DocumentNode, ExecutionResult } from 'graphql';
import { useGraphQl } from './GraphQlClientProvider';

export const createMutation = <TData extends object, TVariables extends object>(
  mutationDocument: DocumentNode,
) => {
  const context = useGraphQl();

  return async (variables: TVariables) => {
    const response = await fetch(context.url(), {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${context.token()}`,
      },
      body: JSON.stringify({
        query: mutationDocument.loc?.source.body,
        variables: variables,
      }),
    });
    const result = (await response.json()) as ExecutionResult<TData>;
    const { data, errors } = result;
    if (errors) {
      throw errors[0];
    }
    return data!;
  };
};
