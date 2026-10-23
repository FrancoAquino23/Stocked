// Evita el parpadeo del loader
export class DelayedLoading {
  private showTimeoutId: ReturnType<typeof setTimeout> | null = null;
  private shownAt: number | null = null;

  constructor(
    private readonly onShow: () => void,
    private readonly showDelayMs = 200,
    private readonly minShownMs = 400,
  ) {}

  // Llamar al empezar la carga
  start(): void {
    this.showTimeoutId = setTimeout(() => {
      this.shownAt = Date.now();
      this.onShow();
    }, this.showDelayMs);
  }

  // Llamar cuando la respuesta ya llegó
  finish(onFinish: () => void): void {
    if (this.showTimeoutId) {
      clearTimeout(this.showTimeoutId);
      this.showTimeoutId = null;
    }

    if (this.shownAt === null) {
      onFinish();
      return;
    }

    const elapsed = Date.now() - this.shownAt;
    const remaining = Math.max(0, this.minShownMs - elapsed);
    setTimeout(onFinish, remaining);
  }
}
