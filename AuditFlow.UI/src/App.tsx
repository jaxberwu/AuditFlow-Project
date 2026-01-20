import { useState, useEffect } from 'react';
import type { AuditSummaryDto, ThreatDetailsDto } from './types';

// Fetch audit summary - 获取审计摘要
async function fetchAuditSummary(): Promise<AuditSummaryDto> {
  const res = await fetch('http://localhost:5002/api/audit/summary', {
    method: 'GET',
    headers: {
      'Accept': 'application/json',
    },
  });
  
  if (!res.ok) {
    throw new Error(`HTTP error! status: ${res.status}`);
  }
  
  return res.json();
}

// Fetch threat details for a specific hostname - 获取指定主机的威胁详细信息
function fetchThreatDetails(hostname: string): Promise<ThreatDetailsDto> {
  return fetch(`http://localhost:5001/api/v1/threats/details/${encodeURIComponent(hostname)}`, {
    method: 'GET',
    headers: {
      'Accept': 'application/json',
    },
  })
    .then(res => {
      if (!res.ok) {
        throw new Error(`HTTP error! status: ${res.status}`);
      }
      return res.json();
    });
}

function ThreatDetailsPanel({ hostname, onClose }: { hostname: string; onClose: () => void }) {
  const [details, setDetails] = useState<ThreatDetailsDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchThreatDetails(hostname)
      .then(data => {
        setDetails(data);
        setLoading(false);
      })
      .catch(err => {
        setError(err.message);
        setLoading(false);
      });
  }, [hostname]);

  if (loading) {
    return (
      <div 
        style={{ 
          position: 'fixed', 
          top: 0, 
          left: 0, 
          right: 0, 
          bottom: 0, 
          backgroundColor: 'rgba(0,0,0,0.5)', 
          display: 'flex', 
          alignItems: 'center', 
          justifyContent: 'center',
          zIndex: 1000
        }}
        onClick={onClose}
      >
        <div 
          style={{ 
            backgroundColor: 'white', 
            padding: '40px', 
            borderRadius: '8px',
            maxWidth: '800px',
            width: '90%'
          }}
          onClick={(e) => e.stopPropagation()}
        >
          <p>Loading threat details...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div 
        style={{ 
          position: 'fixed', 
          top: 0, 
          left: 0, 
          right: 0, 
          bottom: 0, 
          backgroundColor: 'rgba(0,0,0,0.5)', 
          display: 'flex', 
          alignItems: 'center', 
          justifyContent: 'center',
          zIndex: 1000
        }}
        onClick={onClose}
      >
        <div 
          style={{ 
            backgroundColor: 'white', 
            padding: '40px', 
            borderRadius: '8px',
            maxWidth: '800px',
            width: '90%'
          }}
          onClick={(e) => e.stopPropagation()}
        >
          <h2 style={{ color: '#dc2626', marginBottom: '10px' }}>Error</h2>
          <p>{error}</p>
          <button onClick={onClose} style={{ marginTop: '20px', padding: '8px 16px', backgroundColor: '#2563eb', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer' }}>
            Close
          </button>
        </div>
      </div>
    );
  }

  return (
    <div 
      style={{ 
        position: 'fixed', 
        top: 0, 
        left: 0, 
        right: 0, 
        bottom: 0, 
        backgroundColor: 'rgba(0,0,0,0.5)', 
        display: 'flex', 
        alignItems: 'center', 
        justifyContent: 'center',
        zIndex: 1000,
        overflow: 'auto',
        padding: '20px'
      }}
      onClick={onClose}
    >
      <div 
        style={{ 
          backgroundColor: 'white', 
          padding: '24px', 
          borderRadius: '8px',
          maxWidth: '1000px',
          width: '100%',
          maxHeight: '90vh',
          overflow: 'auto',
          boxShadow: '0 4px 6px rgba(0,0,0,0.1)'
        }}
        onClick={(e) => e.stopPropagation()}
      >
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
          <h2 style={{ fontSize: '24px', fontWeight: 'bold', color: '#111827', margin: 0 }}>
            Threat Details: {details?.hostname}
          </h2>
          <button 
            onClick={onClose}
            style={{ 
              padding: '8px 16px', 
              backgroundColor: '#6b7280', 
              color: 'white', 
              border: 'none', 
              borderRadius: '4px', 
              cursor: 'pointer',
              fontSize: '14px'
            }}
          >
            Close
          </button>
        </div>
        
        <div style={{ marginBottom: '16px', padding: '12px', backgroundColor: '#f3f4f6', borderRadius: '4px' }}>
          <strong>Total CVEs: {details?.totalCount}</strong>
          <span style={{ marginLeft: '16px', fontSize: '12px', color: '#6b7280' }}>
            Source: CrowdStrike API
          </span>
        </div>

        {details && details.cves.length > 0 ? (
          <div style={{ overflowX: 'auto' }}>
            <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: '14px' }}>
              <thead style={{ backgroundColor: '#f9fafb' }}>
                <tr>
                  <th style={{ padding: '12px', textAlign: 'left', fontWeight: '500', color: '#6b7280', textTransform: 'uppercase', fontSize: '12px', borderBottom: '1px solid #e5e7eb' }}>
                    CVE ID
                  </th>
                  <th style={{ padding: '12px', textAlign: 'left', fontWeight: '500', color: '#6b7280', textTransform: 'uppercase', fontSize: '12px', borderBottom: '1px solid #e5e7eb' }}>
                    Severity
                  </th>
                  <th style={{ padding: '12px', textAlign: 'left', fontWeight: '500', color: '#6b7280', textTransform: 'uppercase', fontSize: '12px', borderBottom: '1px solid #e5e7eb' }}>
                    Detected Date
                  </th>
                  <th style={{ padding: '12px', textAlign: 'left', fontWeight: '500', color: '#6b7280', textTransform: 'uppercase', fontSize: '12px', borderBottom: '1px solid #e5e7eb' }}>
                    Remediation
                  </th>
                </tr>
              </thead>
              <tbody>
                {details.cves.map((cve, index) => (
                  <tr key={cve.cve_ID} style={{ borderBottom: '1px solid #e5e7eb', backgroundColor: index % 2 === 0 ? 'white' : '#f9fafb' }}>
                    <td style={{ padding: '12px', fontWeight: '500', color: '#111827' }}>
                      {cve.cve_ID}
                    </td>
                    <td style={{ padding: '12px' }}>
                      <span style={{ 
                        padding: '4px 12px', 
                        fontSize: '12px', 
                        fontWeight: '600', 
                        borderRadius: '9999px',
                        backgroundColor: 
                          cve.severity === 'Critical' ? '#fee2e2' :
                          cve.severity === 'High' ? '#fef3c7' :
                          cve.severity === 'Medium' ? '#dbeafe' : '#f0fdf4',
                        color: 
                          cve.severity === 'Critical' ? '#991b1b' :
                          cve.severity === 'High' ? '#92400e' :
                          cve.severity === 'Medium' ? '#1e3a8a' : '#166534'
                      }}>
                        {cve.severity}
                      </span>
                    </td>
                    <td style={{ padding: '12px', color: '#6b7280' }}>
                      {new Date(cve.detectedDate).toLocaleDateString()}
                    </td>
                    <td style={{ padding: '12px', color: '#6b7280' }}>
                      {cve.remediation}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        ) : (
          <p style={{ color: '#6b7280', textAlign: 'center', padding: '40px' }}>
            No CVE threats found for this hostname.
          </p>
        )}
      </div>
    </div>
  );
}

function AuditDashboardContent() {
  // Use useState and useEffect for stable data fetching - 使用 useState 和 useEffect 进行稳定的数据获取
  const [auditSummary, setAuditSummary] = useState<AuditSummaryDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedHostname, setSelectedHostname] = useState<string | null>(null);

  useEffect(() => {
    fetchAuditSummary()
      .then(data => {
        setAuditSummary(data);
        setLoading(false);
      })
      .catch(err => {
        console.error('Error fetching audit summary:', err);
        setError(err.message || 'Failed to fetch audit summary');
        setLoading(false);
      });
  }, []);

  // Loading state - 加载状态
  if (loading) {
    return (
      <div style={{ 
        padding: '40px', 
        backgroundColor: '#f3f4f6', 
        minHeight: '100vh',
        fontFamily: 'system-ui, -apple-system, sans-serif',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center'
      }}>
        <div style={{ textAlign: 'center' }}>
          <div style={{
            width: '48px',
            height: '48px',
            border: '4px solid #e5e7eb',
            borderTop: '4px solid #2563eb',
            borderRadius: '50%',
            animation: 'spin 1s linear infinite',
            margin: '0 auto 16px'
          }}></div>
          <p style={{ color: '#6b7280', fontSize: '16px' }}>Loading audit summary...</p>
        </div>
        <style>{`
          @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
          }
        `}</style>
      </div>
    );
  }

  // Error state - 错误状态
  if (error) {
    return (
      <div style={{ 
        padding: '40px', 
        backgroundColor: '#fef2f2', 
        minHeight: '100vh',
        fontFamily: 'system-ui, -apple-system, sans-serif',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        flexDirection: 'column'
      }}>
        <h2 style={{ fontSize: '24px', fontWeight: 'bold', color: '#dc2626', marginBottom: '10px' }}>
          Error Loading Data
        </h2>
        <p style={{ color: '#991b1b', marginBottom: '20px' }}>{error}</p>
        <button 
          onClick={() => {
            setLoading(true);
            setError(null);
            fetchAuditSummary()
              .then(data => {
                setAuditSummary(data);
                setLoading(false);
              })
              .catch(err => {
                setError(err.message || 'Failed to fetch audit summary');
                setLoading(false);
              });
          }}
          style={{ 
            padding: '10px 20px', 
            backgroundColor: '#2563eb', 
            color: 'white', 
            border: 'none', 
            borderRadius: '8px', 
            cursor: 'pointer',
            fontSize: '16px',
            fontWeight: '500'
          }}
        >
          Retry
        </button>
      </div>
    );
  }

  // No data state - 无数据状态
  if (!auditSummary) {
    return (
      <div style={{ 
        padding: '40px', 
        backgroundColor: '#f3f4f6', 
        minHeight: '100vh',
        fontFamily: 'system-ui, -apple-system, sans-serif',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center'
      }}>
        <p style={{ color: '#6b7280' }}>No audit summary data available.</p>
      </div>
    );
  }

  // Success state - render dashboard - 成功状态 - 渲染仪表板
  return (
    <>
      <div style={{ 
        padding: '20px', 
        backgroundColor: '#f3f4f6', 
        minHeight: '100vh',
        fontFamily: 'system-ui, -apple-system, sans-serif'
      }}>
        <div style={{ 
          maxWidth: '1400px', 
          margin: '0 auto', 
          backgroundColor: 'white', 
          borderRadius: '8px', 
          boxShadow: '0 1px 3px rgba(0,0,0,0.1)', 
          padding: '24px' 
        }}>
          {/* Header - 标题 */}
          <div style={{ borderBottom: '1px solid #e5e7eb', paddingBottom: '20px', marginBottom: '20px' }}>
            <h1 style={{ fontSize: '28px', fontWeight: 'bold', color: '#111827', margin: 0 }}>
              AuditFlow Dashboard
            </h1>
            <p style={{ fontSize: '14px', color: '#6b7280', marginTop: '4px' }}>
              Enterprise Asset Compliance Audit System
            </p>
          </div>

          {/* Stats Cards - 统计卡片 */}
          <div style={{ 
            display: 'grid', 
            gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', 
            gap: '16px', 
            marginBottom: '24px' 
          }}>
            <div style={{ backgroundColor: '#eff6ff', padding: '20px', borderRadius: '8px' }}>
              <div style={{ fontSize: '14px', fontWeight: '500', color: '#2563eb', marginBottom: '8px' }}>
                Total Assets
              </div>
              <div style={{ fontSize: '32px', fontWeight: 'bold', color: '#1e3a8a' }}>
                {auditSummary.totalAssets}
              </div>
            </div>
            <div style={{ backgroundColor: '#fef2f2', padding: '20px', borderRadius: '8px' }}>
              <div style={{ fontSize: '14px', fontWeight: '500', color: '#dc2626', marginBottom: '8px' }}>
                Non-Compliant
              </div>
              <div style={{ fontSize: '32px', fontWeight: 'bold', color: '#991b1b' }}>
                {auditSummary.nonCompliantCount}
              </div>
            </div>
            <div style={{ backgroundColor: '#f0fdf4', padding: '20px', borderRadius: '8px' }}>
              <div style={{ fontSize: '14px', fontWeight: '500', color: '#16a34a', marginBottom: '8px' }}>
                Compliant
              </div>
              <div style={{ fontSize: '32px', fontWeight: 'bold', color: '#166534' }}>
                {auditSummary.compliantCount}
              </div>
            </div>
          </div>

          {/* Compliance Table - 合规性表格 */}
          <div>
            <h2 style={{ fontSize: '20px', fontWeight: '600', color: '#111827', marginBottom: '16px' }}>
              Compliance Status ({auditSummary.complianceItems.length} devices)
            </h2>
            
            <div style={{ overflowX: 'auto' }}>
              <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: '14px' }}>
                <thead style={{ backgroundColor: '#f9fafb' }}>
                  <tr>
                    <th style={{ padding: '12px', textAlign: 'left', fontWeight: '500', color: '#6b7280', textTransform: 'uppercase', fontSize: '12px', borderBottom: '1px solid #e5e7eb' }}>
                      Serial Number
                    </th>
                    <th style={{ padding: '12px', textAlign: 'left', fontWeight: '500', color: '#6b7280', textTransform: 'uppercase', fontSize: '12px', borderBottom: '1px solid #e5e7eb' }}>
                      Hostname
                    </th>
                    <th style={{ padding: '12px', textAlign: 'left', fontWeight: '500', color: '#6b7280', textTransform: 'uppercase', fontSize: '12px', borderBottom: '1px solid #e5e7eb' }}>
                      Purchase Date
                    </th>
                    <th style={{ padding: '12px', textAlign: 'left', fontWeight: '500', color: '#6b7280', textTransform: 'uppercase', fontSize: '12px', borderBottom: '1px solid #e5e7eb' }}>
                      Age (Years)
                    </th>
                    <th style={{ padding: '12px', textAlign: 'left', fontWeight: '500', color: '#6b7280', textTransform: 'uppercase', fontSize: '12px', borderBottom: '1px solid #e5e7eb' }}>
                      Threat Count
                    </th>
                    <th style={{ padding: '12px', textAlign: 'left', fontWeight: '500', color: '#6b7280', textTransform: 'uppercase', fontSize: '12px', borderBottom: '1px solid #e5e7eb' }}>
                      Status
                    </th>
                    <th style={{ padding: '12px', textAlign: 'left', fontWeight: '500', color: '#6b7280', textTransform: 'uppercase', fontSize: '12px', borderBottom: '1px solid #e5e7eb' }}>
                      Action
                    </th>
                    <th style={{ padding: '12px', textAlign: 'left', fontWeight: '500', color: '#6b7280', textTransform: 'uppercase', fontSize: '12px', borderBottom: '1px solid #e5e7eb' }}>
                      Details
                    </th>
                  </tr>
                </thead>
                <tbody>
                  {auditSummary.complianceItems.map((item, index) => (
                    <tr 
                      key={item.sn} 
                      style={{ 
                        borderBottom: '1px solid #e5e7eb',
                        backgroundColor: index % 2 === 0 ? 'white' : '#f9fafb',
                        userSelect: 'text' // Allow text selection - 允许文本选择
                      }}
                    >
                      <td style={{ padding: '16px', fontWeight: '500', color: '#111827' }}>
                        {item.sn}
                      </td>
                      <td style={{ padding: '16px', color: '#6b7280' }}>{item.hostname}</td>
                      <td style={{ padding: '16px', color: '#6b7280' }}>
                        {new Date(item.purchaseDate).toLocaleDateString()}
                      </td>
                      <td style={{ padding: '16px', color: '#6b7280' }}>{item.ageYears}</td>
                      <td style={{ padding: '16px', color: '#6b7280', textAlign: 'center' }}>
                        {item.threatCount > 0 ? (
                          <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '4px' }}>
                            <span style={{ 
                              padding: '4px 8px', 
                              backgroundColor: '#fee2e2', 
                              color: '#991b1b',
                              borderRadius: '4px',
                              fontWeight: '600'
                            }}>
                              {item.threatCount}
                            </span>
                            <span style={{ 
                              fontSize: '10px', 
                              color: '#6b7280',
                              fontStyle: 'italic'
                            }}>
                              Source: CrowdStrike
                            </span>
                          </div>
                        ) : (
                          <span style={{ color: '#9ca3af' }}>0</span>
                        )}
                      </td>
                      <td style={{ padding: '16px' }}>
                        <span style={{ 
                          padding: '4px 12px', 
                          fontSize: '12px', 
                          fontWeight: '600', 
                          borderRadius: '9999px',
                          backgroundColor: item.status === 'Non-Compliant' ? '#fee2e2' : '#f0fdf4',
                          color: item.status === 'Non-Compliant' ? '#991b1b' : '#166534'
                        }}>
                          {item.status}
                        </span>
                      </td>
                      <td style={{ padding: '16px' }}>
                        <span style={{ 
                          padding: '4px 12px', 
                          fontSize: '12px', 
                          fontWeight: '600', 
                          borderRadius: '9999px',
                          backgroundColor: item.action === 'Replace' ? '#fef3c7' : '#e0e7ff',
                          color: item.action === 'Replace' ? '#92400e' : '#3730a3'
                        }}>
                          {item.action}
                        </span>
                      </td>
                      <td style={{ padding: '16px' }}>
                        {item.threatCount > 0 ? (
                          <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '4px' }}>
                            <button 
                              onClick={(e) => {
                                e.preventDefault();
                                e.stopPropagation();
                                setSelectedHostname(item.hostname);
                              }}
                              onMouseDown={(e) => {
                                // Prevent text selection when clicking button - 点击按钮时防止文本选择
                                e.preventDefault();
                              }}
                              style={{ 
                                padding: '6px 12px', 
                                backgroundColor: '#2563eb', 
                                color: 'white', 
                                border: 'none', 
                                borderRadius: '4px', 
                                cursor: 'pointer',
                                fontSize: '12px',
                                userSelect: 'none' // Prevent text selection on button - 防止按钮上的文本选择
                              }}
                            >
                              View CVEs
                            </button>
                            <span style={{ 
                              fontSize: '10px', 
                              color: '#6b7280',
                              fontStyle: 'italic'
                            }}>
                              Source: CrowdStrike
                            </span>
                          </div>
                        ) : (
                          <span style={{ color: '#9ca3af' }}>-</span>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>

      {/* Threat Details Modal - 威胁详情模态框 */}
      {selectedHostname && (
        <ThreatDetailsPanel 
          hostname={selectedHostname} 
          onClose={() => setSelectedHostname(null)} 
        />
      )}
    </>
  );
}

function App() {
  return <AuditDashboardContent />;
}

export default App;
