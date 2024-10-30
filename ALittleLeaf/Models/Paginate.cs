namespace ALittleLeaf.Models
{
    public class Paginate
    {
        public int TotalItems { get; set; } // Tổng số items
        public int PageSize { get; set; } // Tổng số items/trang
        public int CurrentPage { get; set; } // Trang hiện tại
        public int TotalPages { get; set; } // Tổng trang
        public int StartPage { get; set; } // Trang bắt đầu
        public int EndPage { get; set; } // Trang kết thúc
        public int PagesPerGroup { get; set; } = 4; // Số trang hiển thị trong mỗi nhóm

        public Paginate(int totalItems, int page, int pageSize = 12, int pagesPerGroup = 4)
        {
            int totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);

            int currentPage = page;

            // Điều chỉnh start và end theo nhóm
            int startPage = ((currentPage - 1) / pagesPerGroup) * pagesPerGroup + 1;
            int endPage = Math.Min(startPage + pagesPerGroup - 1, totalPages);

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
            PagesPerGroup = pagesPerGroup;
        }
    }

}
