export type DeepPartial<T> = T extends object
  ? { [K in keyof T]?: DeepPartial<T[K]> }
  : T;

function isObject(item: any): boolean {
  return item !== null && typeof item === 'object' && !Array.isArray(item);
}

function deepMergeWithMap(
  target: any,
  source: any,
  visited = new Map<any, any>(),
) {
  if (isObject(target) && isObject(source)) {
    for (const key in source) {
      if (isObject(source[key])) {
        if (!target[key]) {
          target[key] = {};
        }
        if (!visited.has(source[key])) {
          visited.set(source[key], {});
          deepMergeWithMap(target[key], source[key], visited);
        } else {
          target[key] = visited.get(source[key]);
        }
      } else {
        target[key] = source[key];
      }
    }
  }
  return target;
}

export function deepMerge<T>(target: T, source: DeepPartial<T>): T {
  const targetCopy = JSON.parse(JSON.stringify(target));
  const sourceCopy = JSON.parse(JSON.stringify(source));
  return deepMergeWithMap(targetCopy, sourceCopy);
}
