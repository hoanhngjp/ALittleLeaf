import { Link } from 'react-router-dom'

const FOOTER_LOGO_URL = 'https://res.cloudinary.com/dd9umsxtf/image/upload/v1776265291/footerLogo_bpfe7m.webp'

export default function Footer() {
  return (
    <footer>
      <div className="footerWrap">

        <div className="row mainFooter">

          {/* Col 1 — Giới thiệu (wider on lg+) */}
          <div className="col-12 col-md-6 col-lg-4 mb-4 mb-lg-0">
            <div className="footer-contentWrap">
              <h4 className="footer-title">Giới thiệu</h4>
              <div className="footer-content">
                <p>A Little Leaf Tiệm tạp hóa của Tình yêu: dành cho những góc bạn yêu ở nơi được gọi là "Nhà"</p>
              </div>
              <div className="footerLogo">
                <img src={FOOTER_LOGO_URL} alt="A Little Leaf" />
              </div>
            </div>
          </div>

          {/* Col 2 — Liên kết (hidden on mobile — lives in sidebar) */}
          <div className="col-12 col-md-6 col-lg-2 mb-4 mb-lg-0 d-none d-md-block">
            <div className="footer-contentWrap">
              <h4 className="footer-title">Liên kết</h4>
              <div className="footer-content">
                <ul>
                  <li className="items"><Link to="/search">Tìm kiếm</Link></li>
                  <li className="items"><a href="#">Giới thiệu</a></li>
                  <li className="items"><a href="#">Chính sách đổi trả</a></li>
                </ul>
              </div>
            </div>
          </div>

          {/* Col 3 — Showroom (hidden on mobile — lives in sidebar) */}
          <div className="col-12 col-md-6 col-lg-4 mb-4 mb-lg-0 d-none d-md-block">
            <div className="footer-contentWrap">
              <h4 className="footer-title">Showroom</h4>
              <div className="footer-content">
                <ul>
                  <li className="contact">
                    <p><i className="fa-solid fa-location-dot" style={{ fontSize: '20px', marginRight: '6px' }} />
                    212/A51 Nguyễn Trãi, P. Nguyễn Cư Trinh, Quận 1</p>
                  </li>
                  <li className="contact">
                    <p><i className="fa-solid fa-phone" style={{ fontSize: '20px', marginRight: '6px' }} />
                    098.873.55.00</p>
                  </li>
                  <li className="contact">
                    <p><i className="fa-solid fa-envelope" style={{ fontSize: '20px', marginRight: '6px' }} />
                    alittleleaf.homedecor@gmail.com</p>
                  </li>
                </ul>
              </div>
            </div>
          </div>

          {/* Col 4 — Mạng xã hội (hidden on mobile — lives in sidebar) */}
          <div className="col-12 col-md-6 col-lg-2 mb-4 mb-lg-0 d-none d-md-block">
            <div className="footer-contentWrap">
              <h4 className="footer-title">Mạng xã hội</h4>
              <div className="footer-content" style={{ display: 'flex', flexWrap: 'wrap', gap: '4px' }}>
                <div className="wrapSocial"><i className="fa-brands fa-facebook" /></div>
                <div className="wrapSocial"><i className="fa-brands fa-instagram" /></div>
                <div className="wrapSocial"><i className="fa-brands fa-tiktok" /></div>
                <div className="wrapSocial"><i className="fa-brands fa-youtube" /></div>
              </div>
            </div>
          </div>

        </div>

        <div className="footerBottom">
          <p>Copyright © 2024 A Little Leaf. Powered by Alittleleaf</p>
        </div>

      </div>
    </footer>
  )
}
