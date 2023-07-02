// measure text
// thanks https://stackoverflow.com/questions/118241/calculate-text-width-with-javascript
let canvas: HTMLCanvasElement | undefined;
export function GetTextWidth(text: string, font: string) {
  if (!canvas) {
    canvas = document.createElement('canvas');
  }
  const context = canvas.getContext('2d') as CanvasRenderingContext2D;
  context.font = font;
  const metrics = context.measureText(text);
  return metrics.width;
}

export function GetTextHeight(text: string, font: string) {
  if (!canvas) {
    canvas = document.createElement('canvas');
  }
  const context = canvas.getContext('2d') as CanvasRenderingContext2D;
  context.font = font;
  const metrics = context.measureText(text);
  return metrics;
}

export function GetCanvasFont(el = document.body) {
  const fontWeight =
    window.getComputedStyle(el, null).getPropertyValue('font-weight') || 'normal';
  const fontSize =
    window.getComputedStyle(el, null).getPropertyValue('font-size') || '16px';
  const fontFamily =
    window.getComputedStyle(el, null).getPropertyValue('font-family') ||
    'Times New Roman';

  return `${fontWeight} ${fontSize} ${fontFamily}`;
}
