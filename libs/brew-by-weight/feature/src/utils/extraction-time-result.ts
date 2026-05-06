export type ExtractionTimeResult = 'None' | 'Good' | 'Ok' | 'Bad';

export const calculateExtractionTimeResult = (
  isFinished: boolean,
  threshold: {
    good: number;
    bad: number;
  },
  time: number,
  targetTime: number,
) => {
  if (!isFinished) return 'None';
  let diff = targetTime - time;
  if (diff < 0) diff = 0 - diff;
  if (diff < threshold.good) return 'Good';
  if (diff < threshold.bad) return 'Ok';
  return 'Bad';
};
