import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-paginador',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './paginador.component.html',
  styleUrl: './paginador.component.scss'
})
export class PaginadorComponent {
  @Input({ required: true }) currentPage = 1;
  @Input({ required: true }) pageSize = 10;
  @Input({ required: true }) totalCount = 0;
  @Input({ required: true }) totalPages = 0;

  @Output() readonly paginaCambiada = new EventEmitter<number>();

  protected get itemInicio(): number {
    return this.totalCount === 0 ? 0 : (this.currentPage - 1) * this.pageSize + 1;
  }

  protected get itemFin(): number {
    return Math.min(this.currentPage * this.pageSize, this.totalCount);
  }

  protected get paginas(): number[] {
    const maxBotones = 5;
    let inicio = Math.max(1, this.currentPage - Math.floor(maxBotones / 2));
    const fin = Math.min(this.totalPages, inicio + maxBotones - 1);
    inicio = Math.max(1, fin - maxBotones + 1);

    const paginas: number[] = [];
    for (let i = inicio; i <= fin; i++) {
      paginas.push(i);
    }
    return paginas;
  }

  protected irA(pagina: number): void {
    if (pagina < 1 || pagina > this.totalPages || pagina === this.currentPage) {
      return;
    }
    this.paginaCambiada.emit(pagina);
  }

  protected anterior(): void {
    this.irA(this.currentPage - 1);
  }

  protected siguiente(): void {
    this.irA(this.currentPage + 1);
  }
}
